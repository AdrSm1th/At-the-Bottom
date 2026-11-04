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
        public EntityViewModel PlayerViewModel { get; }

        public GameViewModel()
        {
            Level = LevelFactory.CreateTestLevel();
            Tiles = [];
            Entities = [];

            Player = new Player(new Position(5, 5));

            Monster goblin = new Monster(new Position(7, 7));

            PlayerViewModel = new EntityViewModel(Player, TileSize);

            Entities.Add(PlayerViewModel);
            Entities.Add(new EntityViewModel(goblin, TileSize));

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

        public bool MovePlayer(int dx, int dy)
        {
            int x = Player.Position.X + dx;
            int y = Player.Position.Y + dy;

            if (!Level.IsWalkedble(x, y))
                return false;

            if (IsCellOccupied(x, y))
                return false;

            Player.Position = new Position(x, y);
            PlayerViewModel.RefreshPosition();

            return true;
        }

        private bool IsCellOccupied(int x, int y)
        {
            foreach (var entityVM in Entities)
            {
                var position = entityVM.Model.Position;

                if (position.X == x && position.Y == y)
                    return true;
            }

            return false;
        }
    }
}
