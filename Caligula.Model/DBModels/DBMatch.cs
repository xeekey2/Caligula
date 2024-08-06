using System;
using System.Collections.Generic;

namespace Caligula.Model.DBModels
{
    public class DbMatch
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public List<DbParticipant> Participants { get; set; } = new List<DbParticipant>();
        public DbMap Map { get; set; }
        public int MapId { get; set; } // Foreign key to DbMap
        public string Winner { get; set; }
        public string Loser { get; set; }
        public int? Duration { get; set; }
        public DateTime Date { get; set; }
    }

    public class DbParticipant
    {
        public int Id { get; set; }
        public int DbMatchId { get; set; } // Foreign key to DbMatch
        public int ProPlayerId { get; set; } // Foreign key to DbPlayer
        public string Decision { get; set; }
        public int? RatingChange { get; set; }

        public DbMatch DbMatch { get; set; } // Navigation property
        public DbPlayer DbPlayer { get; set; } // Navigation property
    }

    public class DbPlayer
    {
        public int ProPlayerId { get; set; }
        public string Name { get; set; }
        public List<DbParticipant> Participants { get; set; } = new List<DbParticipant>(); // Navigation property
    }

    public class DbMap
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<DbMatch> Matches { get; set; } = new List<DbMatch>(); // Navigation property
    }
}
