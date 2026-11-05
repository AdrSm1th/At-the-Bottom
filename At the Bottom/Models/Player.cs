namespace At_the_Bottom.Models
{
    public class Player(Position pos) : Entity(pos, '@', "Каэль", 20, 4)
    {
        public int Experience { get; private set; } = 0;
        public int Gold { get; private set; } = 0;
        public int Level { get; private set; } = 1;
        public int UpgradePoints { get; private set; } = 0;
        public int ExperienceToNextLevel => Level * 20;

        public void AddRewards(int experience, int gold)
        {
            Experience += Math.Max(0, experience);
            Gold += Math.Max(0, gold);

            while (Experience >= ExperienceToNextLevel)
            {
                Experience -= ExperienceToNextLevel;
                Level++;
                UpgradePoints++;
            }
        }

        public bool TryUpgrade(UpgradeType type)
        {
            if (UpgradePoints <= 0)
            { 
                return false;
            }

            switch (type)
            {
                case UpgradeType.Health:
                    IncreaseMaxHealth(5);
                    break;

                case UpgradeType.Damage:
                    IncreaseDamage(1);
                    break;

                case UpgradeType.Defense:
                    IncreaseDefense(1);
                    break;

                default:
                    return false;
            }

            UpgradePoints--;

            return true;
        }
    }
}
