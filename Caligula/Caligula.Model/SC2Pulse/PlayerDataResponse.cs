using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caligula.Model.SC2Pulse
{
    public class PlayerDataResponse
    {
        public int Realm { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Region { get; set; }
        public int BattlenetId { get; set; }
    }

}
