using At_the_Bottom.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace At_the_Bottom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameViewModel gameViewModel;

        public MainWindow()
        {
            InitializeComponent();

            gameViewModel = new GameViewModel();
            DataContext = gameViewModel;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat)
            {
                return;
            }

            if (DataContext is not GameViewModel vm)
                return;

            if (vm.IsInBattle)
            {
                if (e.Key == Key.Escape)
                    vm.EndBattle();

                e.Handled = true;
                return;
            }

            int dx = 0;
            int dy = 0;

            switch(e.Key)
            {
                case Key.W:
                case Key.Up:
                    dy = -1;
                    break;

                case Key.S:
                case Key.Down:
                    dy = 1;
                    break;

                case Key.A:
                case Key.Left:
                    dx = -1;
                    break;

                case Key.D:
                case Key.Right:
                    dx = 1;
                    break;

                default:
                    return;
            }

            gameViewModel.MovePlayer(dx, dy);
            e.Handled = true;
        }
    }
}