using Caligula.Model.SC2Pulse;

namespace Caligula.Web.Extensions
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
