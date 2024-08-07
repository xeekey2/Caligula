using Caligula.Model.Caligula;
using Caligula.Model.SC2Pulse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caligula.Service.Extensions
{

    public static class ParticipantExtensions
    {
        public static async Task<List<Player>> ToPlayerListAsync(this IEnumerable<Participant> participants)
        {
            var tasks = participants.Select(p => ToPlayerAsync(p.participant)).ToList();
            var players = await Task.WhenAll(tasks);
            return players.ToList();
        }

        private static async Task<Player> ToPlayerAsync(Participant1 participant)
        {
            var name = await participant.playerCharacterId.ToPlayerName();
            return new Player
            {
                Id = participant.playerCharacterId,
                Name = name
            };
        }

        public static async Task<string> ToPlayerName(this int participantId)
        {
            SC2PulseWrapper sc2PulseWrapper = new SC2PulseWrapper("https://sc2pulse.nephest.com/sc2/");
            var response = await sc2PulseWrapper.GetProPlayerName(participantId);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var playerData = JsonConvert.DeserializeObject<GroupResponse>(json);
                var proPlayerName = playerData.characters.FirstOrDefault()?.members.proNickname;

                if (!string.IsNullOrEmpty(proPlayerName))
                {
                    return proPlayerName;
                }
            }

            // If the pro player name is empty or null, or if the request failed, try to get the normal player name
            response = await sc2PulseWrapper.GetNameFromId(participantId);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var playerData = JsonConvert.DeserializeObject<List<PlayerDataResponse>>(json);
                return playerData.FirstOrDefault()?.Name;
            }

            return null;
        }
    }
}
