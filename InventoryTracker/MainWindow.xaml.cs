using InventoryTracker.Models;
using InventoryTracker.UI;
using InventoryTracker.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace InventoryTracker
{
    public partial class MainWindow : Window
    {
        private InventoryDatabase _database;
        private MainViewModel _viewModel;
        private UserService _userService;
        private User _currentUser;

        public MainWindow()
        {
            InitializeComponent();
            InitializeViewModel();
            InitializeDatabase();
            _userService = new UserService();
            DataContext = _viewModel;
        }

        private void InitializeViewModel()
        {
            _viewModel = new MainViewModel();
            _viewModel.InventoryItems = new ObservableCollection<InventoryItem>();
        }

        private void InitializeDatabase()
        {
            try
            {
                _database = new InventoryDatabase();
                //LoadItemsFromDatabase();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while connecting to database: {ex.Message}");
                MessageBox.Show($"Error connecting to database: {ex.Message}");
            }
        }

        private void LoadItemsFromDatabase()
        {
            try
            {
                var itemsFromDb = _database.GetAllItems(_currentUser.UserId);
                _viewModel.InventoryItems.Clear();
                foreach (var item in itemsFromDb)
                {
                    _viewModel.AddItem(item);
                }

                _viewModel.SetOriginalItems();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while loading items from database: {ex.Message}");
                MessageBox.Show($"Error loading items from database: {ex.Message}");
            }
        }


        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (e.EditAction == DataGridEditAction.Commit)
                {
                    var editedItem = e.Row.Item as InventoryItem;
                    var column = e.Column as DataGridTextColumn;
                    if (editedItem != null && column != null)
                    {
                        var editedValue = (e.EditingElement as TextBox).Text;

                        switch (column.Header.ToString())
                        {
                            case "Name":
                                editedItem.Name = editedValue;
                                break;
                            case "Quantity":
                                editedItem.Quantity = int.Parse(editedValue);
                                break;
                            case "Price":
                                editedItem.Price = double.Parse(editedValue);
                                break;
                            default:
                                break;
                        }

                        _database.UpdateItem(editedItem, _currentUser.UserId);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while editing item: {ex.Message}");
                MessageBox.Show($"Error editing item: {ex.Message}");
            }
        }

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
            {
                throw new Exception("Button is null");
            }

            string tag = button.Tag as string;

            // Determine sorting direction
            ListSortDirection direction = ListSortDirection.Ascending;
            if (tag != null && tag.EndsWith("Desc"))
            {
                direction = ListSortDirection.Descending;
            }

            // Determine which column to sort based on button's Tag property
            string propertyName = null;

            if (tag == "NameAsc" || tag == "NameDesc")
            {
                propertyName = "Name";
            }
            else if (tag == "QuantityAsc" || tag == "QuantityDesc")
            {
                propertyName = "Quantity";
            }
            else if (tag == "PriceAsc" || tag == "PriceDesc")
            {
                propertyName = "Price";
            }
            else
            {
                Debug.WriteLine("Unable to sort.");
            }

            // Perform sorting
            if (!string.IsNullOrEmpty(propertyName))
            {
                SortInventoryItems(propertyName, direction);
            }
        }

        // Method to sort InventoryItems collection
        private void SortInventoryItems(string propertyName, ListSortDirection direction)
        {
            // Get the property descriptor for specified property name
            PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(InventoryItem)).Find(propertyName, true);

            if (prop != null)
            {
                // Perform sorting
                _viewModel.InventoryItems = new ObservableCollection<InventoryItem>(
                    direction == ListSortDirection.Ascending ?
                    _viewModel.InventoryItems.OrderBy(x => prop.GetValue(x)) :
                    _viewModel.InventoryItems.OrderByDescending(x => prop.GetValue(x))
                );
            }
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel == null || _viewModel.InventoryItems == null)
                {
                    Debug.WriteLine("ViewModel or ViewModel.InventoryItems is null when trying to add an item.");
                    return;
                }

                InventoryItem newItem = new InventoryItem
                {
                    Name = "New Item",
                    Quantity = 0,
                    Price = 0.0
                };

                _database.AddItem(newItem, _currentUser.UserId);

                newItem = _database.GetItem(newItem.Id);

                _viewModel.AddItem(newItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding item: {ex.Message}");
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = dataGrid.SelectedItem as InventoryItem;
                if (selectedRow != null)
                {
                    _viewModel.DeleteItem(selectedRow.Id);
                    _database.DeleteItem(selectedRow.Id, _currentUser.UserId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while deleting item: {ex.Message}");
                MessageBox.Show($"Error deleting item: {ex.Message}");
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var updatedItem in _viewModel.InventoryItems)
                {
                    _database.UpdateItem(updatedItem, _currentUser.UserId);
                }

                LoadItemsFromDatabase();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while saving changes: {ex.Message}");
                MessageBox.Show($"Error saving changes: {ex.Message}");
            }
        }

        private void CancelChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.InventoryItems.Clear();
                foreach (var item in _viewModel.OriginalItems)
                {
                    _viewModel.AddItem(new InventoryItem
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Quantity = item.Quantity,
                        Price = item.Price
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while canceling changes: {ex.Message}");
                MessageBox.Show($"Error canceling changes: {ex.Message}");
            }
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentUser != null)
                {
                    MessageBox.Show("User already logged in.");
                    return;
                }

                // Create an instance of the UserRegistrationWindow
                UserRegistrationWindow registrationWindow = new UserRegistrationWindow();

                // Show the registration window as a dialog
                bool? result = registrationWindow.ShowDialog();

                // Check the result of the dialog
                if (result == true)
                {
                    // Registration was successful
                    MessageBox.Show("User registered successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Registration was cancelled
                    MessageBox.Show("Registration was cancelled.", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred during registration: {ex.Message}");
                MessageBox.Show($"Error during registration: {ex.Message}");
            }
        }


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_userService == null)
                {
                    Debug.WriteLine("UserService is not initialized.");
                    MessageBox.Show("UserService is not initialized.");
                    return;
                }

                string username = UsernameTextBox.Text;
                string password = PasswordTextBox.Password;

                bool loggedIn = _userService.Login(username, password, out bool isAdmin);
                if (loggedIn)
                {
                    _currentUser = new User { Username = username };
                    _currentUser.UserId = _userService.GetUserID(_currentUser.Username);
                    _userService.CurrentUserId = _currentUser.UserId;
                    SessionInfoTextBlock.Text = $"Logged in as {_currentUser.Username}";

                    _viewModel.IsLoggedIn = true;

                    Debug.WriteLine($"\n\n!!!!!!!!!!!!!!!!! _currentUser.UserId {_currentUser.UserId}\n\n");

                    LoadItemsFromDatabase();
                }
                else
                {
                    MessageBox.Show("Invalid username or password. Please try again.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred during login: {ex.Message}");
                MessageBox.Show($"Error during login: {ex.Message}");
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_userService == null || _currentUser == null)
                {
                    Debug.WriteLine("UserService or CurrentUser is not initialized.");
                    MessageBox.Show("UserService or CurrentUser is not initialized.");
                    return;
                }

                _userService.Logout();

                _currentUser = null;
                _viewModel.IsLoggedIn = false;
                UsernameTextBox.Text = null;
                PasswordTextBox.Password = null;

                SessionInfoTextBlock.Text = "Logged out";

                MessageBox.Show("Logged out successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred during logout: {ex.Message}");
                MessageBox.Show($"Error during logout: {ex.Message}");
            }
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentUser == null)
                {
                    MessageBox.Show("User not logged in.");
                    return;
                }

                // Check admin status again before opening window
                bool isAdmin = _userService.IsAdmin(_currentUser.Username);
                if (isAdmin)
                {
                    var userListWindow = new UserListWindow(isAdmin);
                    userListWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("You do not have permission to access this feature.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while opening user list window: {ex.Message}");
                MessageBox.Show($"Error opening user list window: {ex.Message}");
            }
        }
    }
}
