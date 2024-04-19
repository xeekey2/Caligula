using Caligula.Model.SC2Pulse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caligula.Model.Caligula
{
    public class MatchHistory
    {
        public List<Match> CommonMatches { get; set; }
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
    }

    public class Match
    {
        public List<Player> Players { get; set; }
        public Map Map { get; set; }
        public Participant[] Participants { get; set; }
        public int WinnerPlayerCharacterId { get; set; } 
        public int Duration { get; set; } 
    }

    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Map
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
