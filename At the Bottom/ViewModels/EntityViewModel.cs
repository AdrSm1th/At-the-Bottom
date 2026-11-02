using At_the_Bottom.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace At_the_Bottom.ViewModels
{
    public class EntityViewModel(Entity entity, int tileSize) : ObservableObject
    {
        public double PixelLeft => entity.Position.X * Size;
        public double PixelTop => entity.Position.Y * Size;
        public char Symbol => entity.Symbol;
        public int Size { get; } = tileSize;

        public void RefreshPosition()
        {
            OnPropertyChanged(nameof(PixelLeft));
            OnPropertyChanged(nameof(PixelTop));
        }
    }
}
