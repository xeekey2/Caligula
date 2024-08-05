using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caligula.Model.Caligula;
using SC2PulseModel = Caligula.Model.SC2Pulse.Result;

namespace Caligula.Service.Extensions
{
    public static class MatchExtensions
    {
        public static async Task<List<MatchObject>> ToCaligulaMatchesAsync(this IEnumerable<SC2PulseModel> sc2PulseMatches)
        {
            var tasks = sc2PulseMatches.Select(match => match.ToCaligulaMatchAsync());
            var matchObjects = await Task.WhenAll(tasks);
            return matchObjects.ToList();
        }

        public static async Task<MatchObject> ToCaligulaMatchAsync(this SC2PulseModel sc2PulseMatch)
        {
            var players = await sc2PulseMatch.participants.ToPlayerListAsync();
            var winnerName = await sc2PulseMatch.participants.FirstOrDefault(x => x.participant.decision == "WIN")?.participant.playerCharacterId.ToPlayerName();

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
