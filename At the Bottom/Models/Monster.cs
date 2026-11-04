namespace At_the_Bottom.Models
{
    public class Monster(Position pos)
        : Entity(pos, 'g', "Goblin")
    {
        public int Health { get; set; } = 3;
        public int Damage { get; set; } = 1;
    }
}
