namespace At_the_Bottom.Models
{
    public class Entity(Position pos, char symbol, string name)
    {
        public Position Position { get; set; } = pos;
        public char Symbol { get; } = symbol;
        public string Name { get; } = name;
    }
}
