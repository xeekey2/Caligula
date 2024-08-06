using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Caligula.Model.Caligula;
using Caligula.Model.SC2Pulse;
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
                // Your list of player names
            };

            foreach (var playerName in sc2ProPlayers)
            {
                var player = await GetPlayerInfoAsync(playerName);
                if (player == null) continue;

                var matchHistories = await GetMatchHistoriesDailyAsync(player.ProPlayerId, DateTime.Now.AddMonths(-3), DateTime.Now);

                foreach (var match in matchHistories)
                {
                    if (!await MatchExistsAsync(match.match.Id))
                    {
                        _dbContext.ProLadderMatches.Add(new ProLadderMatch
                        {
                            MatchId = match.match.Id,
                            Date = match.match.Date,
                            Player1Id = match.participants[0].participant.playerCharacterId,
                            Player2Id = match.participants[1].participant.playerCharacterId
                        });
                    }
                }

                await _dbContext.SaveChangesAsync();
            }
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

        private async Task<bool> MatchExistsAsync(int matchId)
        {
            return await _dbContext.ProLadderMatches.AnyAsync(m => m.MatchId == matchId);
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
