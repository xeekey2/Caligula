using Caligula.Model.Caligula;
using SC2PulseModel = Caligula.Model.SC2Pulse.Result;
using Caligula.Web.ApiClients;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caligula.Service.Extensions;

namespace Caligula.Web.Extensions
{
    public static class MatchExtensions
    {
        public static async Task<List<MatchObject>> ToCaligulaMatchesAsync(this IEnumerable<SC2PulseModel> sc2PulseMatches, MatchHistoryApiClient apiClient)
        {
            var tasks = sc2PulseMatches.Select(match => match.ToCaligulaMatchAsync(apiClient));
            var matchObjects = await Task.WhenAll(tasks);
            return matchObjects.ToList();
        }

        public static async Task<MatchObject> ToCaligulaMatchAsync(this SC2PulseModel sc2PulseMatch, MatchHistoryApiClient apiClient)
        {
            var players = await sc2PulseMatch.participants.ToPlayerListAsync(apiClient);
            var winnerId = sc2PulseMatch.participants.FirstOrDefault(x => x.participant.decision == "WIN")?.participant.playerCharacterId ?? 0;
            var winnerName = winnerId > 0 ? await apiClient.GetProPlayerNameAsync(winnerId) : null;

            return new MatchObject
            {
                Players = players,
                Map = sc2PulseMatch.map.ToMap(),
                Winner = winnerName,
                Duration = sc2PulseMatch.match.Duration,
                Date = sc2PulseMatch.match.Date,
                Participants = sc2PulseMatch.participants,
            };
        }
    }
}
