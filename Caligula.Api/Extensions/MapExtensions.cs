using Caligula.Model.Caligula;
using Caligula.Model.SC2Pulse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map = Caligula.Model.SC2Pulse.Map;

namespace Caligula.Service.Extensions
{
    public static class MapExtensions
    {

        public static Caligula.Model.Caligula.Map ToMap(this Map map)
        {
            return new Caligula.Model.Caligula.Map
            {
                Id = map.id,
                Name = map.name
            };
        }

    }
}
