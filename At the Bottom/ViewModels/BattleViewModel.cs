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
