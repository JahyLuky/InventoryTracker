using InventoryTracker.Models;
using System.Windows;

namespace InventoryTracker.UI
{
    public partial class UserRegistrationWindow : Window
    {
        private UserService _userService;

        public UserRegistrationWindow()
        {
            InitializeComponent();
        }

        private void RegistrationSaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the text from TextBox and PasswordBox controls
            string username = RegistrationUsernameTextBox.Text;
            string password = RegistrationPasswordBox.Password;
            string confirmPassword = RegistrationConfirmPasswordBox.Password;

            if (password == confirmPassword)
            {
                _userService = new UserService();
                _userService.CreateUser(username, password, "user");
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Passwords do not match!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegistrationCancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
