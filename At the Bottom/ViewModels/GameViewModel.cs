using At_the_Bottom.Models;
using System.Collections.ObjectModel;

namespace At_the_Bottom.ViewModels
{
    public class GameViewModel
    {
        public Level Level { get; }
        public ObservableCollection<TileViewModel> Tiles { get; }
        public int TileSize { get; } = 32;
        public double CanvasWidth => Level.Width * TileSize;  
        public double CanvasHeight => Level.Height * TileSize;

        public GameViewModel()
        {
            Level = LevelFactory.CreateTestLevel();
            Tiles = new ObservableCollection<TileViewModel>();

            for (int x = 0; x < Level.Width; x++)
            {
                for (int y = 0; y < Level.Height; y++)
                {
                    var tile = Level.GetTile(x, y);
                    if (tile != null)
                    {
                        Tiles.Add(new TileViewModel(tile, TileSize));
                    }
                }
            }
        }
    }
}
