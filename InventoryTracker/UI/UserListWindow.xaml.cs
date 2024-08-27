using InventoryTracker.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace InventoryTracker.UI
{
    /// <summary>
    /// Interaction logic for UserListWindow.xaml
    /// </summary>
    public partial class UserListWindow : Window
    {
        private UserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserListWindow"/> class.
        /// </summary>
        /// <param name="isAdmin">Indicates whether the user is an admin.</param>
        public UserListWindow(bool isAdmin)
        {
            InitializeComponent();
            _userService = new UserService();

            if (isAdmin)
            {
                LoadUsernames();
            }
            else
            {
                MessageBox.Show("Access denied. User is not admin.");
                Close();
            }
        }

        /// <summary>
        /// Loads the usernames from the user service and populates the ListBox.
        /// </summary>
        private void LoadUsernames()
        {
            try
            {
                var usernames = _userService.GetAllUsernames();
                UserListBox.ItemsSource = new ObservableCollection<string>(usernames);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading usernames: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
    }
}
