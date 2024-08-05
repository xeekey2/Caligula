using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caligula.Model.Caligula;
using Caligula.Model.SC2Pulse;
using Caligula.Web.ApiClients;

namespace Caligula.Service.Extensions
{
    public static class ParticipantExtensions
    {
        public static async Task<List<Player>> ToPlayerListAsync(this IEnumerable<Participant> participants, MatchHistoryApiClient apiClient)
        {
            var tasks = participants.Select(p => ToPlayerAsync(p.participant, apiClient)).ToList();
            var players = await Task.WhenAll(tasks);
            return players.ToList();
        }

        private static async Task<Player> ToPlayerAsync(Participant1 participant, MatchHistoryApiClient apiClient)
        {
            var name = await participant.playerCharacterId.ToPlayerNameAsync(apiClient);
            return new Player
            {
                Id = participant.playerCharacterId,
                Name = name
            };
        }

        public static async Task<string> ToPlayerNameAsync(this int participantId, MatchHistoryApiClient apiClient)
        {
            return await apiClient.GetProPlayerNameAsync(participantId);
        }
    }
}
