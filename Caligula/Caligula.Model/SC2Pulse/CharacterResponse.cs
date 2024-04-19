using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caligula.Model.SC2Pulse
{
    public class CharacterResponse
    {
        public CharacterResult[] results { get; set; }
    }

    public class CharacterResult
    {
        public int playerCharacterId { get; set; }
        public string race { get; set; }
        public int games { get; set; }
        public int ratingAvg { get; set; }
        public int ratingMax { get; set; }
        public int ratingLast { get; set; }
        public string leagueTypeLast { get; set; }
        public int globalRankLast { get; set; }
    }


}
