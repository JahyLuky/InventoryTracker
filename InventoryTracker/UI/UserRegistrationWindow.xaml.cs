using System.Windows;

namespace InventoryTracker.UI
{
    public partial class UserRegistrationWindow : Window
    {
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

            // Check if the passwords match
            if (password == confirmPassword)
            {
                // Handle the registration logic here (e.g., save user to database)
                MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Set DialogResult to true to indicate success
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                // Show an error message if passwords do not match
                MessageBox.Show("Passwords do not match!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegistrationCancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Set DialogResult to false to indicate cancellation
            this.DialogResult = false;
            this.Close();
        }
    }
}
