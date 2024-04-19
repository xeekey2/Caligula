using System.Collections.Generic;
using System.Linq;
using Caligula.Model.Caligula;
using SC2PulseModel = Caligula.Model.SC2Pulse.Match;

namespace Caligula.Service.Extensions
{
    public static class MatchExtensions
    {
        public static List<Match> ToCaligulaMatches(this IEnumerable<SC2PulseModel> sc2PulseMatches)
        {
            return sc2PulseMatches.Select(sc2PulseMatch => sc2PulseMatch.ToCaligulaMatch()).ToList();
        }

        public static Match ToCaligulaMatch(this SC2PulseModel sc2PulseMatch)
        {
            return new Match
            {
                Players = sc2PulseMatch.participants.Select(p => new Player { Id = p.participant.playerCharacterId }).ToList(),
                WinnerPlayerCharacterId = sc2PulseMatch.participants.Where(x => x.participant.decision == "WIN").FirstOrDefault().participant.playerCharacterId,
                Duration = sc2PulseMatch.match.duration,
                Map = new Map
                {
                    Id = sc2PulseMatch.map.id,
                    Name = sc2PulseMatch.map.name,
                },
            };
        }
    }
}
