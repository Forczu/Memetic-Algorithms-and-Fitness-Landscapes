using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Model
{
    /// <summary>
    /// Represents the coordinates of a client in the map.
    /// </summary>
    public class Coordinates
    {
        public uint X { get; set; }

        public uint Y { get; set; }

        public Coordinates(uint x, uint y)
        {
            X = x;
            Y = y;
        }
        
        public override bool Equals(object obj)
        {
            Coordinates otherCoord = obj as Coordinates;
            if (otherCoord == null)
                return false;
            if (otherCoord.X != X ||
                otherCoord.Y != Y)
                return false;
            return true;
        }
    }
}
