using System.Windows;
using System.Windows.Input;

namespace Society.View
{
    /// <summary>
    /// Логика взаимодействия для ErorView.xaml
    /// </summary>
    public partial class ErrorView : Window
    {
        public ErrorView(string text1, string text2)
        {
            InitializeComponent();

            TextBlock1.Text = text1;
            TextBlock2.Text = text2;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void pnlControlBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
