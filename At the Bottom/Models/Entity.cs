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
        public int MaxHealth { get; } = maxHealth;
        public int Health { get; private set; } = maxHealth;
        public int Damage { get; } = damage;
        public bool IsAlive => Health > 0;

        public void TakeDamage(int amount)
        {
            if (amount <= 0)
                return;

            Health = Math.Max(0, Health - amount);
        }
    }
}
