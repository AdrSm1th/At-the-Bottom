namespace At_the_Bottom.Models
{
    public class Level
    {
        public Tile[,] Tiles { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Level(int width, int height)
        {
            Tiles = new Tile[width, height];
            Width = width;
            Height = height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++) {
                    Tiles[x, y] = new Tile(TileType.Wall, new Position(x, y));
                }
            }
        }

        public Tile? GetTile(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return null;
            return Tiles[x, y]; 
        }

        public bool IsWalkedble(int x, int y)
        {
            var tile = GetTile(x, y);
            return tile != null && tile.IsWalkable;
        }
    }
}
