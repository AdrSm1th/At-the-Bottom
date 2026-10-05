using At_the_Bottom.Models;

namespace At_the_Bottom.ViewModels
{
    public class EntityViewModel(Entity entity, int tileSize)
    {
        public double PixelLeft => entity.Position.X * Size;
        public double PixelTop => entity.Position.Y * Size;
        public char Symbol => entity.Symbol;
        public int Size { get; } = tileSize;
    }
}
