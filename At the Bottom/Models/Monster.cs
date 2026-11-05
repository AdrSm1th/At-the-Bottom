namespace At_the_Bottom.Models
{
    public class Monster(Position pos)
        : Entity(pos, 'g', "Гоблин", 10, 2)
    {
        public int ExperienceReward { get; } = 10;
        public int GoldReward { get; } = 5;
    }
}
