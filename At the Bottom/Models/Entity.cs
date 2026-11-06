namespace At_the_Bottom.Models
{
    public class Entity(
        Position pos,
        char symbol,
        string name,
        int maxHealth,
        int damage)
    {
        public Position Position { get; set; } = pos;
        public char Symbol { get; } = symbol;
        public string Name { get; } = name;
        public int MaxHealth { get; private set; } = maxHealth;
        public int Health { get; private set; } = maxHealth;
        public int Damage { get; private set; } = damage;
        public int Defense { get; private set; } = 0;
        public bool IsAlive => Health > 0;

        public void TakeDamage(int amount)
        {
            if (amount <= 0)
                return;

            Health = Math.Max(0, Health - amount);
        }

        public void IncreaseMaxHealth(int amount)
        {
            MaxHealth += amount;
            Health += amount;
        }

        public void IncreaseDamage(int amount)
        {
            Damage += amount;
        }

        public void IncreaseDefense(int amount)
        {
            Defense += amount;
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || !IsAlive)
                return;

            Health = Math.Min(MaxHealth, Health + amount);
        }
    }
}
