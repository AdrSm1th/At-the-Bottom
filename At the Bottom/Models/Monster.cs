namespace At_the_Bottom.Models
{
    public class Monster : Entity
    {
        public int ExperienceReward { get; } = 10;
        public int GoldReward { get; } = 5;

        public Monster(
            Position pos,
            char symbol = 'g',
            string name = "Goblin",
            int maxHealth = 10,
            int damage = 2,
            int experienceReward = 10,
            int goldReward = 5)
            : base(pos, symbol, name, maxHealth, damage)
        {
                ExperienceReward = experienceReward;
                GoldReward = goldReward;
        }
    }
}
