using Caligula.Model.SC2Pulse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caligula.Model.Caligula
{ 
    public class PlayerInfo
    {
        public Team[] teams { get; set; }
        public Proplayer proPlayer { get; set; }
        public Match[] matches { get; set; }
        public Member[] members { get; set; }
    }

    public class Proplayer
    {
        public ProplayerExtended proPlayerExtended { get; set; }
    }

    public class ProplayerExtended
    {
        public int id { get; set; }
        public int aligulacId { get; set; }
        public string nickname { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public string birthday { get; set; }
        public int earnings { get; set; }
        public DateTime updated { get; set; }
        public int version { get; set; }
    }


    public class Team
    {
        public int rating { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public int ties { get; set; }
        public int points { get; set; }
        public int id { get; set; }
        public int legacyId { get; set; }
        public int divisionId { get; set; }
        public int season { get; set; }
        public string region { get; set; }
        public League league { get; set; }
        public string tierType { get; set; }
        public int globalRank { get; set; }
        public int regionRank { get; set; }
        public int leagueRank { get; set; }
        public DateTime lastPlayed { get; set; }
        public Member[] members { get; set; }
        public int leagueId { get; set; }
        public int globalTeamCount { get; set; }
        public int regionTeamCount { get; set; }
        public int leagueTeamCount { get; set; }
        public string leagueType { get; set; }
        public string queueType { get; set; }
        public string teamType { get; set; }
    }

    public class League
    {
        public string type { get; set; }
        public string queueType { get; set; }
        public string teamType { get; set; }
    }

    public class Member
    {
        public int terranGamesPlayed { get; set; }
        public int protossGamesPlayed { get; set; }
        public int zergGamesPlayed { get; set; }
        public int randomGamesPlayed { get; set; }
        public Character character { get; set; }
        public Account account { get; set; }
        public int proId { get; set; }
        public string proNickname { get; set; }
        public string proTeam { get; set; }
        public bool restrictions { get; set; }
    }


}
