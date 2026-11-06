using At_the_Bottom.Models;
using At_the_Bottom.Services;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

        public int PlayerLevel => Player.Level;
        public int UpgradePoints => Player.UpgradePoints;

        public int PlayerMaxHealth => Player.MaxHealth;
        public int PlayerHealth => Player.Health;
        public int PlayerDamage => Player.Damage;
        public int PlayerDefense => Player.Defense;
        public int PlayerPotions => Player.Potions;

        public int PlayerExperience => Player.Experience;
        public int PlayerExperienceToNextLevel => Player.ExperienceToNextLevel;
        public int PlayerGold => Player.Gold;


        private bool isLevelUpOpen;
        public bool IsLevelUpOpen
        {
            get => isLevelUpOpen;
            private set => SetProperty(ref isLevelUpOpen, value);
        }

        private bool isInBattle = false;
        public bool IsInBattle
        {
            get => isInBattle;
            private set => SetProperty(ref isInBattle, value);
        }


        private BattleViewModel? currentBattle;
        public BattleViewModel? CurrentBattle
        {
            get => currentBattle;
            private set => SetProperty(ref currentBattle, value);
        }

        private bool isGameOver;
        public bool IsGameOver
        {
            get => isGameOver;
            private set => SetProperty(ref isGameOver, value);
        }

        private readonly BattleService battleService = new();

        public GameViewModel()
        {
            Level = LevelFactory.CreateTestLevel();
            Tiles = [];
            Entities = [];

            AttackCommand = new RelayCommand(Attack);
            DefendCommand = new RelayCommand(Defend);
            UsePotionCommand = new RelayCommand(UsePotion);

            UpgradeHealthCommand = new RelayCommand(
                () => ApplyUpgrade(UpgradeType.Health));

            UpgradeDamageCommand = new RelayCommand(
                () => ApplyUpgrade(UpgradeType.Damage));

            UpgradeDefenseCommand = new RelayCommand(
                () => ApplyUpgrade(UpgradeType.Defense));

            Player = new Player(new Position(5, 5));

            Monster goblin = new (new Position(7, 7));
            Monster goblin2 = new (new Position(3, 3));
            Monster goblin3 = new (new Position(2, 5));

            PlayerViewModel = new EntityViewModel(Player, TileSize);

            Entities.Add(PlayerViewModel);
            Entities.Add(new EntityViewModel(goblin, TileSize));
            Entities.Add(new EntityViewModel(goblin2, TileSize));
            Entities.Add(new EntityViewModel(goblin3, TileSize));

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

            if (IsGameOver || IsLevelUpOpen)
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
        public IRelayCommand UsePotionCommand { get; }
        public IRelayCommand UpgradeHealthCommand { get; }
        public IRelayCommand UpgradeDamageCommand { get; }
        public IRelayCommand UpgradeDefenseCommand { get; }

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

        private void UsePotion()
        {
            if (!IsInBattle || CurrentBattle == null)
                return;

            if (!Player.TryUsePotion())
                return;

            CurrentBattle.RefreshHealth();
            RefreshPlayerStats();

            EnemyTurn(CurrentBattle);
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
            RefreshPlayerStats();

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

                Player.AddRewards(
                    enemy.ExperienceReward,
                    enemy.GoldReward
                );
            }
            RefreshPlayerStats();

            EndBattle();

            OnPropertyChanged(nameof(PlayerLevel));
            OnPropertyChanged(nameof(UpgradePoints));

            IsLevelUpOpen = Player.UpgradePoints > 0;
        }

        private void ApplyUpgrade(UpgradeType type)
        {
            if (!Player.TryUpgrade(type))
                return;

            OnPropertyChanged(nameof(UpgradePoints));
            OnPropertyChanged(nameof(PlayerMaxHealth));
            OnPropertyChanged(nameof(PlayerDamage));
            OnPropertyChanged(nameof(PlayerDefense));

            IsLevelUpOpen = Player.UpgradePoints > 0;

            RefreshPlayerStats();
        }

        private void RefreshPlayerStats()
        {
            OnPropertyChanged(nameof(PlayerHealth));
            OnPropertyChanged(nameof(PlayerMaxHealth));
            OnPropertyChanged(nameof(PlayerLevel));

            OnPropertyChanged(nameof(PlayerExperience));
            OnPropertyChanged(nameof(PlayerExperienceToNextLevel));

            OnPropertyChanged(nameof(PlayerGold));
            OnPropertyChanged(nameof(PlayerDamage));
            OnPropertyChanged(nameof(PlayerDefense));
            OnPropertyChanged(nameof(UpgradePoints));
            OnPropertyChanged(nameof(PlayerPotions));
        }
    }
}
