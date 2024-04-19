using Caligula.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Caligula.Model;
using Caligula.Model.Caligula;
using Caligula.Model.SC2Pulse;
using Caligula.Service.Extensions;

namespace Caligula.Api
{
    public class MatchHistoryService
    {
        private readonly SC2PulseWrapper Sc2PulseWrapper = new SC2PulseWrapper("https://sc2pulse.nephest.com/sc2/");

        public async Task<MatchHistory> GetMatchHistoryForTwoPlayersAsync(string player1Name, string player2Name)
        {
            var player1Id = await GetPlayerId(player1Name);
            var player2Id = await GetPlayerId(player2Name);

            var player1Matches = await GetMatchHistoryAsync(player1Id);
            var player2Matches = await GetMatchHistoryAsync(player2Id);

            var commonMatches = FindCommonMatches(player1Matches, player2Matches, player1Id, player2Id);

            return new MatchHistory
            {
                CommonMatches = commonMatches.ToCaligulaMatches(),
                PlayerOne = new Player { Id = player1Id },
                PlayerTwo = new Player { Id = player2Id }
            };
        }

        private List<Caligula.Model.SC2Pulse.Match> FindCommonMatches(MatchHistoryResponse player1Matches, MatchHistoryResponse player2Matches, int player1Id, int player2Id)
        {
            return player1Matches.matches
                .Where(m1 => player2Matches.matches.Any(m2 =>
                    m1.participants.Any(p1 => p1.participant.playerCharacterId == player1Id) &&
                    m2.participants.Any(p2 => p2.participant.playerCharacterId == player2Id) &&
                    m1 == m2))
                .ToList();
        }

        private async Task<int> GetPlayerId(string playerName)
        {
            var response = await Sc2PulseWrapper.GetPlayerIdAsync(playerName);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var playerJSON = JsonConvert.DeserializeObject<SearchResponse>(json);
                return playerJSON.Results.OrderBy(x => x.ratingMax).FirstOrDefault().members.character.accountId;
            }
            return -1;
        }

        private async Task<MatchHistoryResponse> GetMatchHistoryAsync(int playerId)
        {
            var response = await Sc2PulseWrapper.GetPlayerMatchHistoryAsync(playerId);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MatchHistoryResponse>(json);
            }
            return null;
        }
    }
}