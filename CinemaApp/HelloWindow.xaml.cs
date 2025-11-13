using System.Windows;

namespace CinemaApp
{
    /// <summary>
    /// Логика взаимодействия для HelloWindow.xaml
    /// </summary>
    public partial class HelloWindow : Window
    {
        public HelloWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var login = UserSession.Instance.CurrentUser?.Login;

            if (login is not null)
                HelloTextBlock.Text = $"Привет, {login}!";
            else
                HelloTextBlock.Text = "Произошла ошибка";
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Instance.Clear();
            Close();
        }
    }
}
