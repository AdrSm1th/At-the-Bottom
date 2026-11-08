using At_the_Bottom.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace At_the_Bottom.ViewModels
{
    public class BattleViewModel : ObservableObject
    {
        public Player Player { get; }
        public Monster Enemy { get; }

        public string PlayerName => Player.Name;
        public string EnemyName => Enemy.Name;

        public int PlayerHealth => Player.Health;
        public int PlayerMaxHealth => Player.MaxHealth;

        public int EnemyHealth => Enemy.Health;
        public int EnemyMaxHealth => Enemy.MaxHealth;

        private EnemyIntent currentIntent = EnemyIntent.NormalAttack;
        public EnemyIntent CurrentIntent
        {
            get => currentIntent;

            private set
            {
                if (SetProperty(ref currentIntent, value))
                    OnPropertyChanged(nameof(EnemyIntentText));
            }
        }
        public string EnemyIntentText => CurrentIntent switch
        {
            EnemyIntent.NormalAttack => "Обычная атака",
            EnemyIntent.HeavyAttack => "Мощная атака (x2)",
            EnemyIntent.Rest => "Передышка",
            _ => "Неизвестно"
        };

        private bool counterAttackReady;
        public bool CounterAttackReady
        {
            get => counterAttackReady;

            private set
            {
                if (SetProperty(ref counterAttackReady, value))
                    OnPropertyChanged(nameof(CounterAttackText));
            }
        }
        public string CounterAttackText =>
            CounterAttackReady
                ? "Контратака готова! Следующая атака усилена."
                : "";
        public void PrepareCounterAttack()
        {
            CounterAttackReady = true;
        }
        public void ConsumeCounterAttack()
        {
            CounterAttackReady = false;
        }

        public void AdvanceIntent()
        {
            CurrentIntent = CurrentIntent switch
            {
                EnemyIntent.NormalAttack => EnemyIntent.HeavyAttack,
                EnemyIntent.HeavyAttack => EnemyIntent.Rest,
                EnemyIntent.Rest => EnemyIntent.NormalAttack,
                _ => EnemyIntent.NormalAttack
            };
        }

        public BattleViewModel(Player player, Monster enemy)
        {
            Player = player;
            Enemy = enemy;
        }

        public void RefreshHealth()
        {
            OnPropertyChanged(nameof(PlayerHealth));
            OnPropertyChanged(nameof(EnemyHealth));
        }
    }
}
