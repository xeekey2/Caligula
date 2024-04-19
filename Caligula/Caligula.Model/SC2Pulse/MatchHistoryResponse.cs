using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caligula.Model.SC2Pulse
{

    public class MatchHistoryResponse
    {
        public Team[] teams { get; set; }
        public Linkeddistinctcharacter[] linkedDistinctCharacters { get; set; }
        public Stat[] stats { get; set; }
        public Proplayer proPlayer { get; set; }
        public Match[] matches { get; set; }
    }

    public class Proplayer
    {
        public Proplayer1 proPlayer { get; set; }
        public Proteam proTeam { get; set; }
        public Link[] links { get; set; }
    }

    public class Proplayer1
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

    public class Proteam
    {
        public string name { get; set; }
        public string shortName { get; set; }
        public int id { get; set; }
        public int aligulacId { get; set; }
        public DateTime updated { get; set; }
        public string uniqueName { get; set; }
    }

    public class Link
    {
        public int proPlayerId { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public DateTime updated { get; set; }
        public string serviceUserId { get; set; }
        public bool _protected { get; set; }
    }

    public class History
    {
        public object[] dateTime { get; set; }
        public int?[] leagueRank { get; set; }
        public int[] games { get; set; }
        public int[] rating { get; set; }
        public int[] teamType { get; set; }
        public int?[] regionTeamCount { get; set; }
        public int[] leagueType { get; set; }
        public int?[] wins { get; set; }
        public int?[] leagueTeamCount { get; set; }
        public int[] queueType { get; set; }
        public int?[] globalRank { get; set; }
        public int[] season { get; set; }
        public int?[] tier { get; set; }
        public int?[] regionRank { get; set; }
        public int?[] globalTeamCount { get; set; }
        public int[] teamId { get; set; }
        public string[] race { get; set; }
    }

    public class Team
    {
        public int rating { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public int ties { get; set; }
        public int id { get; set; }
        public string legacyId { get; set; }
        public int divisionId { get; set; }
        public int season { get; set; }
        public string region { get; set; }
        public League league { get; set; }
        public int globalRank { get; set; }
        public int regionRank { get; set; }
        public int leagueRank { get; set; }
        public DateTime lastPlayed { get; set; }
        public Member[] members { get; set; }
        public int globalTeamCount { get; set; }
        public int regionTeamCount { get; set; }
        public int leagueTeamCount { get; set; }
        public int queueType { get; set; }
        public int leagueType { get; set; }
        public int teamType { get; set; }
        public int tierType { get; set; }
    }

    public class League
    {
        public int type { get; set; }
        public int queueType { get; set; }
        public int teamType { get; set; }
    }

    public class Member
    {
        public int zergGamesPlayed { get; set; }
        public Character character { get; set; }
        public Account account { get; set; }
        public int proId { get; set; }
        public string proNickname { get; set; }
        public string proTeam { get; set; }
        public int terranGamesPlayed { get; set; }
        public int protossGamesPlayed { get; set; }
        public int randomGamesPlayed { get; set; }
    }

    public class Linkeddistinctcharacter
    {
        public int leagueMax { get; set; }
        public int ratingMax { get; set; }
        public int totalGamesPlayed { get; set; }
        public Previousstats previousStats { get; set; }
        public Currentstats currentStats { get; set; }
        public Members members { get; set; }
    }

    public class Character1
    {
        public int realm { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public int accountId { get; set; }
        public string region { get; set; }
        public int battlenetId { get; set; }
    }

    public class Account1
    {
        public string battleTag { get; set; }
        public int id { get; set; }
        public string partition { get; set; }
        public object hidden { get; set; }
    }

    public class Stat
    {
        public Stats stats { get; set; }
        public Previousstats1 previousStats { get; set; }
        public Currentstats1 currentStats { get; set; }
    }

    public class Stats
    {
        public int id { get; set; }
        public int playerCharacterId { get; set; }
        public int queueType { get; set; }
        public int teamType { get; set; }
        public string race { get; set; }
        public int ratingMax { get; set; }
        public int leagueMax { get; set; }
        public int gamesPlayed { get; set; }
    }

    public class Previousstats1
    {
        public int? rating { get; set; }
        public int? gamesPlayed { get; set; }
        public object rank { get; set; }
    }

    public class Currentstats1
    {
        public int? rating { get; set; }
        public int? gamesPlayed { get; set; }
        public object rank { get; set; }
    }

    public class Match
    {
        public Match1 match { get; set; }
        public Map map { get; set; }
        public Participant[] participants { get; set; }
    }

    public class Match1
    {
        public DateTime date { get; set; }
        public string type { get; set; }
        public int id { get; set; }
        public int mapId { get; set; }
        public string region { get; set; }
        public DateTime updated { get; set; }
        public int duration { get; set; }
    }

    public class Map
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Participant
    {
        public Participant1 participant { get; set; }
        public string twitchVodUrl { get; set; }
        public bool? subOnlyTwitchVod { get; set; }
    }

    public class Participant1
    {
        public int matchId { get; set; }
        public int playerCharacterId { get; set; }
        public int teamId { get; set; }
        public DateTime teamStateDateTime { get; set; }
        public string decision { get; set; }
        public int ratingChange { get; set; }
    }

    public class Character2
    {
        public int realm { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public int accountId { get; set; }
        public string region { get; set; }
        public int battlenetId { get; set; }
    }

    public class Account2
    {
        public string battleTag { get; set; }
        public int id { get; set; }
        public string partition { get; set; }
        public object hidden { get; set; }
    }

    public class League2
    {
        public int type { get; set; }
        public int queueType { get; set; }
        public int teamType { get; set; }
        public object id { get; set; }
        public object seasonId { get; set; }
    }

}
