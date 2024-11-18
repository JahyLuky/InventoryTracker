using InventoryTracker.Models;
using System.Windows;

namespace YourNamespace
{
    public partial class UserListWindow : Window
    {
        // Property to store the selected user
        public User SelectedUser { get; private set; }
        private UserService _userService;

        public UserListWindow()
        {
            InitializeComponent();
            _userService = new UserService();
            LoadUserList();
        }

        // Method to load the list of users into the DataGrid
        private void LoadUserList()
        {
            try
            {
                // Simulate fetching users from a database
                List<User> users = _userService.GetAllUsers();

                // Bind the user list to the DataGrid
                UserListView.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}");
            }
        }

        // Event handler for Confirm button
        private void ConfirmSelection_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected user from the DataGrid
            SelectedUser = UserListView.SelectedItem as User;

            if (SelectedUser == null)
            {
                MessageBox.Show("Please select a user.");
                return;
            }

            DialogResult = true;
            Close();
        }

        // Event handler for Cancel button
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
