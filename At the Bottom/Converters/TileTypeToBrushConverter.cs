using At_the_Bottom.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace At_the_Bottom.Converters
{
    public class TileTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TileType type)
            {
                return type switch
                {
                    TileType.Wall => Brushes.DarkGray,
                    TileType.Floor => Brushes.LightGray,
                    TileType.Stairs => Brushes.Gold,
                    _ => Brushes.Magenta
                };
            }

            return Brushes.Magenta;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
