using AuthLibrary.Service;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AuthLibrary.Contexts;

namespace CinemaApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AuthService _authService = new(new CinemaUserDbContext());
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            string login = UserLoginTextBox.Text;
            string password = UserPasswordBox.Password;

            var result = await _authService.RegistrateUserAsync(login,password);
            if (result)
                MessageBox.Show("Успех!","Вы зарегистрированы" , MessageBoxButton.OK,MessageBoxImage.Information);
            else
                MessageBox.Show("Ошибка!","Не удалось зарегистрироваться", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async void AuthorizationButton_Click(object sender, RoutedEventArgs e)
        {
            string login = UserLoginTextBox.Text;
            string password = UserPasswordBox.Password;

            var user = await _authService.AuthUserAsync(login,password);

            if (user is not null)
            {
                UserSession.Instance.SetCurrentUser(user);

                HelloWindow window = new();
                Hide();
                window.ShowDialog();
                Show();
            }
            else
                MessageBox.Show("Не удалось авторизироваться", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}