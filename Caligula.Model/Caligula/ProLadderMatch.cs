using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caligula.Model.Caligula
{
    public class ProLadderMatch
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public DateTime Date { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public string Map { get; set; }
        public string Winner { get; set; }
        public int MmrChange { get; set; }
    }
}

