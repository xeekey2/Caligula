using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Caligula.Model.Caligula;
using Caligula.Model.DBModels;
using Caligula.Model.SC2Pulse;
using Caligula.Service.Entity;
using Caligula.Service.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Caligula.Web.ApiClients
{
    public class MatchHistoryApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _dbContext;

        public MatchHistoryApiClient(HttpClient httpClient, ApplicationDbContext dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task RunDailyMatchHistoryUpdateAsync()
        {
            List<string> sc2ProPlayers = new List<string>
            {
                "Serral"
            };

            foreach (var playerName in sc2ProPlayers)
            {
                var player = await GetPlayerInfoAsync(playerName);
                if (player == null) continue;

                var matchHistories = await GetMatchHistoriesDailyAsync(player.ProPlayerId, DateTime.Now.AddMonths(-3), DateTime.Now);

                foreach (var match in matchHistories)
                {
                    // Check if the match already exists using the MatchId
                    if (!await _dbContext.Matches.AnyAsync(m => m.MatchId == match.match.Id))
                    {
                        var winnerParticipant = match.participants.FirstOrDefault(p => p.participant.decision == "WIN");
                        var loserParticipant = match.participants.FirstOrDefault(p => p.participant.decision != "WIN");

                        if (winnerParticipant != null && loserParticipant != null)
                        {
                            // Check if the Map exists using Map's name or another unique identifier
                            var map = await _dbContext.Maps.FirstOrDefaultAsync(m => m.Name == match.map.name);
                            if (map == null)
                            {
                                // Insert new Map if it does not exist
                                map = new DbMap
                                {
                                    Name = match.map.name
                                };
                                _dbContext.Maps.Add(map);
                                await _dbContext.SaveChangesAsync();
                            }

                            var playerIds = match.participants.Select(p => p.participant.playerCharacterId).Distinct();
                            foreach (var playerId in playerIds)
                            {
                                await EnsurePlayerExistsAsync(player);
                            }

                            var dbMatch = new DbMatch
                            {
                                MatchId = match.match.Id,
                                Date = match.match.Date,
                                Winner = await GetProPlayerNameAsync(winnerParticipant.participant.playerCharacterId),
                                Loser = string.IsNullOrEmpty(await GetProPlayerNameAsync(loserParticipant.participant.playerCharacterId)) ? "unknown" : await GetProPlayerNameAsync(loserParticipant.participant.playerCharacterId),
                                Duration = match.match.Duration,
                                MapId = map.Id,
                                Participants = match.participants.Select(async p => new DbParticipant
                                {
                                    ProPlayerId = await GetProPlayerIdAsync(p.participant.playerCharacterId),
                                    Decision = p.participant.decision,
                                    RatingChange = p.participant.ratingChange
                                }).Select(t => t.Result).ToList()
                            };

                            _dbContext.Matches.Add(dbMatch);

                            try
                            {
                                await _dbContext.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                    }
                }
            }
        }



        private async Task<bool> MatchExistsAsync(int matchId)
        {
            return await _dbContext.Matches.AnyAsync(m => m.MatchId == matchId);
        }


        public async Task<MatchHistory> GetMatchHistoryForTwoPlayersAsync(string player1Name, string player2Name)
        {
            var player1 = await GetPlayerInfoAsync(player1Name);
            var player2 = await GetPlayerInfoAsync(player2Name);

            var player1Matches = await GetMatchHistoriesDailyAsync(player1.ProPlayerId, DateTime.Now.AddMonths(-3), DateTime.Now.AddDays(-1));
            var player2Matches = await GetMatchHistoriesDailyAsync(player2.ProPlayerId, DateTime.Now.AddMonths(-3), DateTime.Now.AddDays(-1));

            var commonMatches = FindCommonMatches(player1Matches, player2Matches, player1, player2);

            var commonMatchTasks = commonMatches.Select(m => m.ToCaligulaMatchAsync());
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


        private async Task<Player> GetPlayerInfoAsync(string playerName)
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
            return accounts.Select(x => x.members.account.id).ToList();
        }

        private async Task<List<Result>> GetMatchHistoriesDailyAsync(int playerId, DateTime endDate, DateTime startDate)
        {
            List<Result> matchHistories = new List<Result>();

            for (DateTime date = startDate; date >= endDate; date = date.AddDays(-1))
            {
                string dateString = date.ToString("yyyy-MM-ddTHH:mm:ss.ffffff'Z'", CultureInfo.InvariantCulture);
                string requestUrl = $"/proplayer/matchhistory/{playerId}/{dateString}";

                var response = await _httpClient.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<Result>>(json);

                    if (results?.Count > 0)
                    {
                        matchHistories.AddRange(results);
                    }
                }
            }

            return matchHistories;
        }


        private async Task EnsurePlayerExistsAsync(Player player)
        {
            if (!await _dbContext.Players.AnyAsync(p => p.ProPlayerId == player.ProPlayerId))
            {
                var playerName = await GetProPlayerNameAsync(player.Id);
                if (playerName != null)
                {
                    var dbPlayer = new DbPlayer
                    {
                        ProPlayerId = player.ProPlayerId, // Assuming ProPlayerId is the same as playerCharacterId
                        Name = playerName
                    };

                    _dbContext.Database.OpenConnection();
                    try
                    {
                        _dbContext.Players.Add(dbPlayer);
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (Exception ef)
                    {
                        throw;
                    }
                    finally
                    {
                        _dbContext.Database.CloseConnection();
                    }
                }
            }
        }

        public async Task<string> GetProPlayerNameAsync(int playerCharacterId)
        {
            var response = await _httpClient.GetAsync($"/proplayername/{playerCharacterId}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var playerData = JsonConvert.DeserializeObject<GroupResponse>(jsonString);
                return playerData?.characters.FirstOrDefault()?.members.proNickname;
            }

            response = await _httpClient.GetAsync($"/playername/{playerCharacterId}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var playerData = JsonConvert.DeserializeObject<List<PlayerDataResponse>>(jsonString);
                return playerData?.FirstOrDefault()?.Name;
            }

            return null;
        }


        public async Task<int> GetProPlayerIdAsync(int playerCharacterId)
        {
            var response = await _httpClient.GetAsync($"/proplayerid/{playerCharacterId}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var playerData = JsonConvert.DeserializeObject<SearchResponse>(jsonString);
                return playerData.members.proId;
            }

            return -1;
        }
    }
}
