using System;
using System.Collections.Generic;
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

namespace Caligula.Service
{
    public class PlayerComparisonService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly HttpClient _httpClient;

        public PlayerComparisonService(ApplicationDbContext dbContext, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task<MatchHistory> GetMatchHistoryForTwoPlayersAsync(string player1Name, string player2Name)
        {
            var player1 = await GetPlayerInfoAsync(player1Name);
            var player2 = await GetPlayerInfoAsync(player2Name);

            if (player1 == null || player2 == null)
            {
                throw new Exception("One or both players not found.");
            }

            var commonMatches = await GetMatchesForPlayersAsync(player1.Ids, player2.Ids);
            var caligulaMatches = await commonMatches.ToCaliMatchesAsync();

            var history = new MatchHistory
            {
                CommonMatches = caligulaMatches.OrderByDescending(x => x.Date).ToList(),
                PlayerOne = player1,
                PlayerTwo = player2,
                PlayerOneTotalWins = caligulaMatches.Count(x => x.Winner == player1.Name),
                PlayerTwoTotalWins = caligulaMatches.Count(x => x.Winner == player2.Name),
                PlayerOneTotalLosses = caligulaMatches.Count(x => x.Winner != player1.Name && x.Winner != null),
                PlayerTwoTotalLosses = caligulaMatches.Count(x => x.Winner != player2.Name && x.Winner != null),
            };

            return history;
        }

        private async Task<Player> GetPlayerInfoAsync(string playerName)
        {
            var response = await _httpClient.GetAsync($"/playerid/{playerName}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var playerStats = JsonConvert.DeserializeObject<List<SearchResponse>>(jsonString);
                var selectedPlayer = playerStats?
                    .Where(p => p.currentStats != null && p.currentStats.rating != null)
                    .OrderByDescending(p => p.currentStats.rating)
                    .FirstOrDefault();

                if (selectedPlayer?.members != null)
                {
                    var member = selectedPlayer.members;
                    return new Player
                    {
                        Id = member.character.id,
                        Ids = await GetProPlayerIdsAsync(member.proId),
                        ProPlayerId = member.proId,
                        Name = member.proNickname
                    };
                }
            }

            return null;
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

        private async Task<List<int>> GetProPlayerIdsAsync(int proPlayerId)
        {
            var response = await _httpClient.GetAsync($"/proplayer/ids/{proPlayerId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var accounts = JsonConvert.DeserializeObject<List<SearchResponse>>(json);
                return accounts.Select(x => x.members.account.id).ToList();
            }

            return new List<int>();
        }

        private async Task<List<DbMatch>> GetMatchesForPlayersAsync(List<int> player1Ids, List<int> player2Ids)
        {
            return await _dbContext.Matches
                .Include(m => m.Participants)
                .ThenInclude(p => p.DbPlayer)
                .Include(m => m.Map)
                .Where(m => m.Participants.Any(p => player1Ids.Contains(p.PlayerId)) && m.Participants.Any(p => player2Ids.Contains(p.PlayerId)))
                .ToListAsync();
        }

        private async Task<List<DbMatch>> GetMatchesForPlayersAsync(int player1Id, int player2Id)
        {
            return await _dbContext.Matches
                .Include(m => m.Participants)
                .ThenInclude(p => p.DbPlayer)
                .Include(m => m.Map)
                .Where(m => m.Participants.Any(p => p.PlayerId == player1Id) && m.Participants.Any(p => p.PlayerId == player2Id))
                .ToListAsync();
        }

    }
}
