namespace At_the_Bottom.Models
{
    public class Tile (TileType type, Position position)
    {
        public TileType Type { get; set; } = type;
        public bool IsVisible { get; set; } = false;
        public bool IsExplored { get; set; } = false;
        public Position Position { get; set; } = position;
        public bool IsWalkable => Type != TileType.Wall;
        public bool IsEnemy { get; set; } = false;
    }
}
