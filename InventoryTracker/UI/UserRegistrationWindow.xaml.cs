using InventoryTracker.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

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
            if (SetRoleForUser.SelectedItem == null)
            {
                MessageBox.Show("Please select a role.");
                return;
            }

            string username = RegistrationUsernameTextBox.Text;
            string password = RegistrationPasswordBox.Password;
            string confirmPassword = RegistrationConfirmPasswordBox.Password;

            if (password == confirmPassword)
            {
                _userService = new UserService();
                ComboBoxItem selectedItem = SetRoleForUser.SelectedItem as ComboBoxItem;
                string selectedRole = selectedItem.Content.ToString();
                bool UserCreated = _userService.CreateUser(username, password, selectedRole);

                Debug.WriteLine($"!!!!!!!!!! Username: {username}, UserCreated: {UserCreated}, role: {selectedRole}");

                if (UserCreated)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Username already in use!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
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
