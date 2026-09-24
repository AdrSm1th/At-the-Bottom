using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace At_the_Bottom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var level = Models.LevelFactory.CreateTestLevel();

            for (int y = 0; y < level.Height; y++)
            {
                var sb = new System.Text.StringBuilder();
                for (int x = 0; x < level.Width; x++)
                {
                    var tile = level.GetTile(x, y);
                    char c = tile?.Type switch
                    {
                        Models.TileType.Wall => '#',
                        Models.TileType.Floor => '.',
                        Models.TileType.Stairs => '>',
                        _ => '?'
                    };
                    sb.Append(c);
                }
                System.Diagnostics.Debug.WriteLine(sb.ToString());
            }
        }
    }
}