using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caligula.Model.SC2Pulse
{
    public class GroupResponse
    {
        public GroupCharacter[] characters { get; set; }
    }

    public class GroupCharacter
    {
        public GroupMembers members { get; set; }
    }


    public class GroupMembers
    {
        public int zergGamesPlayed { get; set; }
        public int proId { get; set; }
        public string proNickname { get; set; }
        public string proTeam { get; set; }
    }

}
