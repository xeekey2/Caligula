using Caligula.Model.Caligula;
using Caligula.Model.SC2Pulse;
using Caligula.Web.Extensions;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http;

namespace Caligula.Web.ApiClients
{
    public class MatchHistoryApiClient(HttpClient _httpClient)
    {
        public async Task<MatchHistory> GetMatchHistoryForTwoPlayersAsync(string player1Name, string player2Name)
        {
            var player1 = await GetPlayerInfoAsync(player1Name);
            var player2 = await GetPlayerInfoAsync(player2Name);

            var player1Matches = await GetMatchHistoriesDailyAsync(player1.ProPlayerId, DateTime.Now.AddMonths(-3), DateTime.Now.AddDays(-1));
            var player2Matches = await GetMatchHistoriesDailyAsync(player2.ProPlayerId, DateTime.Now.AddMonths(-3), DateTime.Now.AddDays(-1));

            var commonMatches = FindCommonMatches(player1Matches, player2Matches, player1, player2);

            var commonMatchTasks = commonMatches.Select(m => m.ToCaligulaMatchAsync(this));
            var caligulaMatches = await Task.WhenAll(commonMatchTasks);

            var (playerOne, playerTwo) = DeterminePlayerOrientation(commonMatches, player1, player2);

            var history = new MatchHistory
            {
                CommonMatches = caligulaMatches.OrderByDescending(x => x.Date).ToList(),
                PlayerOne = playerOne,
                PlayerTwo = playerTwo,
                PlayerOneTotalWins = caligulaMatches.Count(x => x.Winner == playerOne.Name),
                PlayerTwoTotalWins = caligulaMatches.Count(x => x.Winner == playerTwo.Name),
                PlayerOneTotalLosses = caligulaMatches.Count(x => x.Winner != playerOne.Name && x.Winner != null),
                PlayerTwoTotalLosses = caligulaMatches.Count(x => x.Winner != playerTwo.Name && x.Winner != null),
            };

            return history;
        }

        private (Player playerOne, Player playerTwo) DeterminePlayerOrientation(List<Result> commonMatches, Player initialPlayerOne, Player initialPlayerTwo)
        {
            int playerOneMatches = commonMatches.Count(m => m.participants.Any(p => p.participant.playerCharacterId == initialPlayerOne.Id));
            int playerTwoMatches = commonMatches.Count(m => m.participants.Any(p => p.participant.playerCharacterId == initialPlayerTwo.Id));

            if (playerOneMatches == 0 && playerTwoMatches > 0)
            {
                return (initialPlayerTwo, initialPlayerOne);
            }
            return (initialPlayerOne, initialPlayerTwo);
        }

        private List<Result> FindCommonMatches(List<Result> player1Matches, List<Result> player2Matches, Player player1, Player player2)
        {
            var commonMatches = FindCommonMatchesHelper(player1Matches, player2Matches, player1.Ids);

            if (!commonMatches.Any())
            {
                commonMatches = FindCommonMatchesHelper(player2Matches, player1Matches, player2.Ids);
            }

            return commonMatches;
        }

        private List<Result> FindCommonMatchesHelper(List<Result> baseMatches, List<Result> compareMatches, List<int> playerIds)
        {
            var baseMatchIds = new HashSet<int>(baseMatches.Select(m => m.match.Id));

            var matches = compareMatches
                .Where(m => m.participants.Any(p => playerIds.Contains(p.participant.playerCharacterId)) && baseMatchIds.Contains(m.match.Id))
                .ToList();

            var uniqueMatches = matches
                .GroupBy(m => m.match.Id)
                .Select(g => g.First())
                .ToList();

            return uniqueMatches;
        }

        public async Task<Player> GetPlayerInfoAsync(string playerName)
        {
            var response = await _httpClient.GetAsync($"/playerid/{playerName}");
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var playerStats = JsonConvert.DeserializeObject<List<SearchResponse>>(jsonString);
            if (playerStats == null || !playerStats.Any())
            {
                return null;
            }

            var selectedPlayer = playerStats
                .Where(p => p.currentStats != null && p.currentStats.rating != null)
                .OrderByDescending(p => p.currentStats.rating)
                .FirstOrDefault();

            if (selectedPlayer?.members != null)
            {
                var member = selectedPlayer.members;
                return new Player
                {
                    Id = member.account.id,
                    Ids = await GetProPlayerIdsAsync(member.proId),
                    ProPlayerId = member.proId,
                    Name = member.proNickname
                };
            }

            return null;
        }

        private async Task<List<int>> GetProPlayerIdsAsync(int proPlayerId)
        {
            var response = await _httpClient.GetAsync($"/proplayer/ids/{proPlayerId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var accounts = JsonConvert.DeserializeObject<List<SearchResponse>>(json);
            return accounts.Select(x => x.members.character.id).ToList();
        }

        private async Task<List<Result>> GetMatchHistoriesDailyAsync(int playerId, DateTime endDate, DateTime startDate)
        {
            List<Result> matchHistories = new List<Result>();

            try
            {
                for (DateTime date = startDate; date >= endDate; date = date.AddDays(-1)) // Adjusting to reduce the number of requests
                {
                    string dateString = date.ToString("yyyy-MM-ddTHH:mm:ss.ffffff'Z'", CultureInfo.InvariantCulture);
                    string requestUrl = $"/proplayer/matchhistory/{playerId}/{dateString}";

                    // Log the request URL for debugging
                    Console.WriteLine($"Request URL: {requestUrl}");

                    try
                    {
                        var response = await _httpClient.GetAsync(requestUrl);

                        // If the response is not successful, log the status code and reason
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                            continue;
                        }

                        var json = await response.Content.ReadAsStringAsync();
                        var results = JsonConvert.DeserializeObject<List<Result>>(json);

                        if (results?.Count > 0)
                        {
                            matchHistories.AddRange(results);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log exceptions that occur within the loop
                        Console.WriteLine($"Exception while processing date {dateString}: {ex.Message}");
                        continue; // Continue with the next iteration
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any unexpected exceptions that occur
                Console.WriteLine($"Unexpected exception: {ex.Message}");
                throw; // Re-throw the exception after logging
            }

            return matchHistories;
        }


        public async Task<string> GetProPlayerNameAsync(int participantId)
        {
            var response = await _httpClient.GetAsync($"/proplayername/{participantId}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var playerData = JsonConvert.DeserializeObject<GroupResponse>(jsonString);
                return playerData?.characters.FirstOrDefault()?.members.proNickname;
            }

            response = await _httpClient.GetAsync($"/playername/{participantId}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var playerData = JsonConvert.DeserializeObject<List<PlayerDataResponse>>(jsonString);
                return playerData?.FirstOrDefault()?.Name;
            }

            return null;
        }
    }
}
