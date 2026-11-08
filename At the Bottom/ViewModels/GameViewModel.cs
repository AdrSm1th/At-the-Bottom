using At_the_Bottom.Models;
using At_the_Bottom.Services;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace At_the_Bottom.ViewModels
{
    public class GameViewModel : ObservableObject
    {
        public Level Level { get; private set; }
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

        public IRelayCommand AttackCommand { get; }
        public IRelayCommand DefendCommand { get; }
        public IRelayCommand UsePotionCommand { get; }
        public IRelayCommand UpgradeHealthCommand { get; }
        public IRelayCommand UpgradeDamageCommand { get; }
        public IRelayCommand UpgradeDefenseCommand { get; }


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

        private bool bossDefeated = false;
        public bool BossDefeated
        {
            get => bossDefeated;
            private set => SetProperty(ref bossDefeated, value);
        }

        private int floorNumber = 1;
        public int FloorNumber
        {
            get => floorNumber;
            private set => SetProperty(ref floorNumber, value);
        }

        private readonly BattleService battleService = new();

        public GameViewModel()
        {
            Level = new Level(24, 15);
            Tiles = [];
            Entities = [];
            Player = new Player(new Position(0, 0));
            PlayerViewModel = new EntityViewModel(Player, TileSize);
            LoadFloor(LevelFactory.GenerateRandomFloor(), FloorNumber);

            AttackCommand = new RelayCommand(Attack);
            DefendCommand = new RelayCommand(Defend);
            UsePotionCommand = new RelayCommand(UsePotion);

            UpgradeHealthCommand = new RelayCommand(
                () => ApplyUpgrade(UpgradeType.Health));

            UpgradeDamageCommand = new RelayCommand(
                () => ApplyUpgrade(UpgradeType.Damage));

            UpgradeDefenseCommand = new RelayCommand(
                () => ApplyUpgrade(UpgradeType.Defense));
        }

        public bool MovePlayer(int dx, int dy)
        {

            if (IsGameOver || IsLevelUpOpen)
                return false;

            int x = Player.Position.X + dx;
            int y = Player.Position.Y + dy;

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

            Tile? tile = Level.GetTile(x, y);
            if (tile?.Type == TileType.Stairs)
            {
                if (!BossDefeated)
                    return false;

                GoToNextFloor();
                return true;
            }


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

        private void Attack()
        {
            if (CurrentBattle == null)
                return;

            BattleViewModel battle = CurrentBattle;

            int bonusDamage = battle.CounterAttackReady
                ? (Player.Damage + 1) / 2
                : 0;

            // Герой атакует
            battleService.Attack(battle.Player, battle.Enemy, damageBonus: bonusDamage);
            battle.ConsumeCounterAttack();
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
            switch (battle.CurrentIntent)
            {
                case EnemyIntent.NormalAttack:
                case EnemyIntent.HeavyAttack:
                    int multiplier = battle.CurrentIntent == EnemyIntent.HeavyAttack
                            ? 2
                            : 1;

                // Враг атакует
                battleService.Attack(
                battle.Enemy,
                battle.Player,
                isDefending: playerDefending,
                damageMultiplier: multiplier);

                // Подготовка контратаки
                if (playerDefending && battle.Player.IsAlive)
                {
                    battle.PrepareCounterAttack();
                }

                break;

                case EnemyIntent.Rest:
                    // Противник пропускает ход
                    break;
            }


            battle.RefreshHealth();
            RefreshPlayerStats();

            if (!battle.Player.IsAlive)
            {
                IsGameOver = true;
                EndBattle();
                return;
            }

            battle.AdvanceIntent();
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

                if (enemy is Boss)
                {
                    BossDefeated = true;
                }
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

        private void GoToNextFloor()
        {
            int nextFloor = FloorNumber;

            int monsterCountNext = Math.Min(
                3 + (nextFloor - 1) / 2, 8);

            GeneratedFloor generated =
                LevelFactory.GenerateRandomFloor(monsterCount : monsterCountNext);

            LoadFloor(generated, nextFloor);

            FloorNumber++;
        }

        private void LoadFloor(GeneratedFloor generated, int floor)
        {
            Level = generated.Level;

            Tiles.Clear();

            for (int x = 0; x < Level.Width; x++)
            {
                for (int y = 0; y < Level.Height; y++)
                {
                    Tile? tile = Level.GetTile(x, y);

                    if (tile != null)
                        Tiles.Add(new TileViewModel(tile, TileSize));
                }
            }

            Entities.Clear();

            Player.Position = generated.PlayerSpawn;
            PlayerViewModel.RefreshPosition();

            Entities.Add(PlayerViewModel);

            foreach (Position pos in generated.MonsterSpawns)
            {
                Monster monster = MonsterFactory.CreateRandom(pos, floor);

                Entities.Add(
                    new EntityViewModel(monster, TileSize)
                );
            }

            Boss boss = new(generated.BossSpawn, floor);

            Entities.Add(
                new EntityViewModel(boss, TileSize)
            );

            BossDefeated = false;

            OnPropertyChanged(nameof(Level));
            OnPropertyChanged(nameof(CanvasWidth));
            OnPropertyChanged(nameof(CanvasHeight));
        }

    }
}
