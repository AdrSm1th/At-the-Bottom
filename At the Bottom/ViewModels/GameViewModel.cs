using At_the_Bottom.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using At_the_Bottom.Services;
using CommunityToolkit.Mvvm.Input;
using System.Linq;

namespace At_the_Bottom.ViewModels
{
    public class GameViewModel : ObservableObject
    {
        public Level Level { get; }
        public ObservableCollection<TileViewModel> Tiles { get; }
        public int TileSize { get; } = 32;
        public double CanvasWidth => Level.Width * TileSize;  
        public double CanvasHeight => Level.Height * TileSize;

        public Player Player { get; }
        public ObservableCollection<EntityViewModel> Entities { get; }
        public EntityViewModel PlayerViewModel { get; }

        public bool IsInBattle
        {
            get => isInBattle;
            private set => SetProperty(ref isInBattle, value);
        }
        public BattleViewModel? CurrentBattle
        {
            get => currentBattle;
            private set => SetProperty(ref currentBattle, value);
        }

        public bool IsGameOver
        {
            get => isGameOver;
            private set => SetProperty(ref isGameOver, value);
        }

        private bool isInBattle = false;
        private BattleViewModel? currentBattle;
        private readonly BattleService battleService = new();
        private bool isGameOver;

        public GameViewModel()
        {
            Level = LevelFactory.CreateTestLevel();
            Tiles = [];
            Entities = [];

            AttackCommand = new RelayCommand(Attack);
            DefendCommand = new RelayCommand(Defend);

            Player = new Player(new Position(5, 5));

            Monster goblin = new Monster(new Position(7, 7));
            Monster goblin2 = new Monster(new Position(3, 3));

            PlayerViewModel = new EntityViewModel(Player, TileSize);

            Entities.Add(PlayerViewModel);
            Entities.Add(new EntityViewModel(goblin, TileSize));
            Entities.Add(new EntityViewModel(goblin2, TileSize));

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

            if (IsGameOver)
                return false;

            if (!Level.IsWalkedble(x, y))
                return false;

            Entity? target = GetEntityAt(x, y);

            if (target is Monster monster)
            {
                StartBattle(monster);
                return false;
            }

            if (target != null)
                return false;

            Player.Position = new Position(x, y);
            PlayerViewModel.RefreshPosition();

            return true;
        }

        public void StartBattle(Monster enemy)
        {
            CurrentBattle = new BattleViewModel(Player, enemy);
            IsInBattle = true;
        }

        public void EndBattle()
        {
            IsInBattle = false;
            CurrentBattle = null;
        }

        public IRelayCommand AttackCommand { get; }
        public IRelayCommand DefendCommand { get; }

        private void Attack()
        {
            if (CurrentBattle == null)
                return;

            BattleViewModel battle = CurrentBattle;

            // Герой атакует
            battleService.Attack(battle.Player, battle.Enemy);
            battle.RefreshHealth();

            if (!battle.Enemy.IsAlive)
            {
                WinBattle(battle.Enemy);
                return;
            }

            EnemyTurn(battle);
        }

        private void Defend()
        {
            if (CurrentBattle == null)
            {
                return;
            }

            EnemyTurn(CurrentBattle, true);
        }

        private void EnemyTurn(
            BattleViewModel battle,
            bool playerDefending = false)
        {
            // Враг атакует
            battleService.Attack(
                battle.Enemy,
                battle.Player,
                playerDefending
            );

            battle.RefreshHealth();

            if (!battle.Player.IsAlive)
            {
                IsGameOver = true;
                EndBattle();
            }
        }

        private Entity? GetEntityAt(int x, int y)
        {
            foreach (var entityVM in Entities)
            {
                Entity entity = entityVM.Model;

                if (entity.Position.X == x &&
                    entity.Position.Y == y)
                {
                    return entity;
                }
            }

            return null;
        }

        private void WinBattle(Monster enemy)
        {
            var enemyVM = Entities.FirstOrDefault(
                e => ReferenceEquals(e.Model, enemy)
            );

            if (enemyVM != null)
            {
                Entities.Remove(enemyVM);
            }

            EndBattle();
        }
    }
}
