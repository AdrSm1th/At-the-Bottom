namespace At_the_Bottom.Models
{
    public class LevelFactory
    {
        public static Level CreateTestLevel()
        {
            int width = 10;
            int height = 10;
            Level level = new(width, height);

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++) 
                {
                    var tile = level.GetTile(x, y);
                    if (tile != null)
                    {
                        level.Tiles[x, y].Type = TileType.Floor;
                    }
                }
            }

            level.Tiles[width - 2, height - 2].Type = TileType.Stairs;
            level.Tiles[width - 4, height - 4].Type = TileType.Wall;

            return level;
        }
    }
}
