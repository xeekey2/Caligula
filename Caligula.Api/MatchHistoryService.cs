//using Caligula.Service;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Caligula.Model;
//using Caligula.Model.Caligula;
//using Caligula.Model.SC2Pulse;
//using Caligula.Service.Extensions;
//using System.Globalization;

//namespace Caligula.Api
//{
//    public class MatchHistoryService
//    {
//        private readonly SC2PulseWrapper Sc2PulseWrapper = new SC2PulseWrapper("https://sc2pulse.nephest.com/sc2/");


//        public async Task<MatchHistory> GetMatchHistoryForTwoPlayersAsync(string player1Name, string player2Name)
//        {
//            var player1 = await GetPlayerInfo(player1Name);
//            var player2 = await GetPlayerInfo(player2Name);

//            var player1Matches = await GetMatchHistoriesDailyAsync(player1.ProPlayerId, DateTime.Now.AddMonths(-3), DateTime.Now.AddDays(1));
//            var player2Matches = await GetMatchHistoriesDailyAsync(player2.ProPlayerId, DateTime.Now.AddMonths(-3), DateTime.Now.AddDays(1));

//            var commonMatches = FindCommonMatches(player1Matches, player2Matches, player1, player2);

//            var commonMatchTasks = commonMatches.Select(m => m.ToCaligulaMatchAsync());
//            var caligulaMatches = await Task.WhenAll(commonMatchTasks);

//            var (playerOne, playerTwo) = DeterminePlayerOrientation(commonMatches, player1, player2);

//            var history = new MatchHistory
//            {
//                CommonMatches = caligulaMatches.OrderByDescending(x => x.Date).ToList(),
//                PlayerOne = playerOne,
//                PlayerTwo = playerTwo,
//                PlayerOneTotalWins = caligulaMatches.Count(x => x.Winner == playerOne.Name),
//                PlayerTwoTotalWins = caligulaMatches.Count(x => x.Winner == playerTwo.Name),
//                PlayerOneTotalLosses = caligulaMatches.Count(x => x.Winner != playerOne.Name && x.Winner != null),
//                PlayerTwoTotalLosses = caligulaMatches.Count(x => x.Winner != playerTwo.Name && x.Winner != null),
//            };

//            return history;
//        }


//        private (Player playerOne, Player playerTwo) DeterminePlayerOrientation(List<Result> commonMatches, Player initialPlayerOne, Player initialPlayerTwo)
//        {
//            int playerOneMatches = commonMatches.Count(m => m.participants.Any(p => p.participant.playerCharacterId == initialPlayerOne.Id));
//            int playerTwoMatches = commonMatches.Count(m => m.participants.Any(p => p.participant.playerCharacterId == initialPlayerTwo.Id));

//            if (playerOneMatches == 0 && playerTwoMatches > 0)
//            {
//                return (initialPlayerTwo, initialPlayerOne);
//            }
//            return (initialPlayerOne, initialPlayerTwo);
//        }


//        private List<Result> FindCommonMatches(List<Result> player1Matches, List<Result> player2Matches, Player player1, Player player2)
//        {
//            var commonMatches = FindCommonMatchesHelper(player1Matches, player2Matches, player1.Ids);

//            if (!commonMatches.Any())
//            {
//                commonMatches = FindCommonMatchesHelper(player2Matches, player1Matches, player2.Ids);
//            }

//            return commonMatches;
//        }

//        private List<Result> FindCommonMatchesHelper(List<Result> baseMatches, List<Result> compareMatches, List<int> playerIds)
//        {
//            var baseMatchIds = new HashSet<int>(baseMatches.Select(m => m.match.Id));

//            var matches = compareMatches
//                .Where(m => m.participants.Any(p => playerIds.Contains(p.participant.playerCharacterId)) && baseMatchIds.Contains(m.match.Id))
//                .ToList();

//            var uniqueMatches = matches
//                .GroupBy(m => m.match.Id)
//                .Select(g => g.First())
//                .ToList();

//            return uniqueMatches;
//        }



//        private async Task<Player> GetPlayerInfo(string playerName)
//        {
//            var response = await Sc2PulseWrapper.GetPlayerIdAsync(playerName);
//            if (!response.IsSuccessStatusCode)
//            {
//                return null;
//            }

//            var json = await response.Content.ReadAsStringAsync();
//            var playerStats = JsonConvert.DeserializeObject<List<SearchResponse>>(json);
//            var selectedPlayer = playerStats.OrderByDescending(p => p.currentStats.rating).FirstOrDefault();

//            if (selectedPlayer?.members != null) 
//            {
//                var member = selectedPlayer.members;
//                return new Player { Id = selectedPlayer.members.account.id, Ids = await GetProPlayerIds(selectedPlayer.members.proId), ProPlayerId = selectedPlayer.members.proId ,Name = selectedPlayer.members.proNickname };
//            }

//            return null;
//        }

//        private async Task<List<int>> GetProPlayerIds(int proPlayerId)
//        {
//            var response = await Sc2PulseWrapper.GetAllIdsFromProPlayerId(proPlayerId);
//            if (!response.IsSuccessStatusCode)
//            {
//                return null;
//            }

//            var json = await response.Content.ReadAsStringAsync();
//            var accounts = JsonConvert.DeserializeObject<List<SearchResponse>>(json);
//            return accounts.Select(x => x.members.character.id).ToList();
//        }


//        private async Task<List<Result>> GetMatchHistoriesDailyAsync(int playerId, DateTime endDate, DateTime startDate)
//        {
//            List<Result> matchHistories = new List<Result>();

//            for (DateTime date = startDate; date >= endDate; date = date.AddDays(-1))
//            {
//                string dateString = date.ToString("yyyy-MM-ddTHH:mm:ss.ffffff'Z'", CultureInfo.InvariantCulture);
//                var response = await Sc2PulseWrapper.GetGroupedProPlayerMatchHistoryAsync(playerId, dateString);
//                if (response.IsSuccessStatusCode)
//                {
//                    try
//                    {
//                        var json = await response.Content.ReadAsStringAsync();
//                        var results = JsonConvert.DeserializeObject<List<Result>>(json);
//                        if (results.Count > 0)
//                        {
//                            matchHistories.AddRange(results);
//                        }
//                    }
//                    catch (Exception e)
//                    {
//                        continue;
//                    }
//                }
//            }
//            return matchHistories;
//        }
//    }
//}