using At_the_Bottom.Models;

namespace At_the_Bottom.ViewModels
{
    public class TileViewModel
    {
        public TileType Type { get; }
        public double PixelLeft { get; }
        public double PixelTop { get; }
        public double Size { get; }

        public TileViewModel(Tile tile, int tileSize)
        {
            Type = tile.Type;
            PixelLeft = tile.Position.X * tileSize;
            PixelTop = tile.Position.Y * tileSize;
            Size = tileSize;
        }
    }
}
