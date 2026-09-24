using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace At_the_Bottom.Models
{
    public class Position(int x, int y)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;

        public override string ToString() => $"({X} {Y})";

        public override bool Equals(object? obj)
        {
            return obj is Position other && X == other.X && Y == other.Y;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}
