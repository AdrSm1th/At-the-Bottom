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
        public Player Player { get; }
        public ObservableCollection<EntityViewModel> Entities { get; }

        public GameViewModel()
        {
            Level = LevelFactory.CreateTestLevel();
            Tiles = new ObservableCollection<TileViewModel>();
            Entities = new ObservableCollection<EntityViewModel>();

            Player = new Player(new Position(5, 5));

            Entities.Add(new EntityViewModel(Player, TileSize));

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
