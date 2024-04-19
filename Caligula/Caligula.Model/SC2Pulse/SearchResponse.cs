namespace Caligula.Model.SC2Pulse
{

    public class SearchResponse
    {
        public SearchResult[] Results { get; set; }
    }
    public class SearchResult
    {
        public string leagueMax { get; set; }
        public int ratingMax { get; set; }
        public int totalGamesPlayed { get; set; }
        public Previousstats previousStats { get; set; }
        public Currentstats currentStats { get; set; }
        public Members members { get; set; }
    }

    public class Previousstats
    {
        public int rating { get; set; }
        public int gamesPlayed { get; set; }
        public int rank { get; set; }
    }

    public class Currentstats
    {
        public int rating { get; set; }
        public int gamesPlayed { get; set; }
        public int rank { get; set; }
    }

    public class Members
    {
        public int terranGamesPlayed { get; set; }
        public int protossGamesPlayed { get; set; }
        public int zergGamesPlayed { get; set; }
        public int randomGamesPlayed { get; set; }
        public Character character { get; set; }
        public Account account { get; set; }
        public Clan clan { get; set; }
        public int proId { get; set; }
        public string proNickname { get; set; }
        public string proTeam { get; set; }
        public bool restrictions { get; set; }
    }

    public class Character
    {
        public int realm { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public int accountId { get; set; }
        public string region { get; set; }
        public int battlenetId { get; set; }
    }

    public class Account
    {
        public string battleTag { get; set; }
        public int id { get; set; }
        public string partition { get; set; }
        public bool hidden { get; set; }
    }

    public class Clan
    {
        public string tag { get; set; }
        public int id { get; set; }
        public string region { get; set; }
        public string name { get; set; }
        public int members { get; set; }
        public int activeMembers { get; set; }
        public int avgRating { get; set; }
        public string avgLeagueType { get; set; }
        public int games { get; set; }
    }

}