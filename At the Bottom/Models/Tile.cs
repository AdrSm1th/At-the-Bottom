using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace At_the_Bottom.Models
{
    public class Tile (TyleType type, Position position)
    {
        public TyleType Type { get; set; } = type;
        public bool IsVisible { get; set; } = false;
        public bool IsExplored { get; set; } = false;
        public Position Position { get; set; } = position;
        public bool IsWalkable => Type != TyleType.Wall;
    }
}
