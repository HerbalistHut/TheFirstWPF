using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class World
    {
        private List<Location> _locations = new List<Location>();

        internal void AddLocation(Location location)
        {
            _locations.Add(location);
        }

        public Location LocationAt (int x, int y)
        {
            foreach (Location location in _locations)
            {
                if (location.XCoordinate == x && location.YCoordinate == y)
                {
                    return location;
                }
            }
            return null;
        }
    }
}
