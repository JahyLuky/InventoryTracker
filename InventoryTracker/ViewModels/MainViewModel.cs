using InventoryTracker.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace InventoryTracker.ViewModels
{
    /// <summary>
    /// The view model for the main window, managing the application's state and inventory items.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _isLoggedIn;
        private bool _isAdmin;

        /// <summary>
        /// Gets or sets a value indicating whether the user is logged in.
        /// </summary>
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged(nameof(IsLoggedIn));
                OnPropertyChanged(nameof(CanAddDeleteItem)); // Notify that CanAddDeleteItem property may have changed
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the logged in user has admin privileges.
        /// </summary>
        public bool IsAdmin
        {
            get { return _isAdmin; }
            set
            {
                _isAdmin = value;
                OnPropertyChanged(nameof(IsAdmin));
                OnPropertyChanged(nameof(CanShowUserButton)); // Notify that CanShowUserButton property may have changed
            }
        }

        private ObservableCollection<InventoryItem> _inventoryItems;

        /// <summary>
        /// Gets or sets the collection of inventory items.
        /// </summary>
        public ObservableCollection<InventoryItem> InventoryItems
        {
            get { return _inventoryItems; }
            set
            {
                _inventoryItems = value;
                OnPropertyChanged(nameof(InventoryItems));
                SetOriginalItems(); // Update _originalItems whenever InventoryItems changes
            }
        }

        private ObservableCollection<InventoryItem> _originalItems;

        /// <summary>
        /// Gets the original collection of inventory items.
        /// </summary>
        public ObservableCollection<InventoryItem> OriginalItems
        {
            get { return _originalItems; }
            private set
            {
                _originalItems = value;
                OnPropertyChanged(nameof(OriginalItems));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            _inventoryItems = new ObservableCollection<InventoryItem>();
            SetOriginalItems(); // Initialize _originalItems
        }

        /// <summary>
        /// Sets the original items collection to a copy of the current inventory items.
        /// </summary>
        public void SetOriginalItems()
        {
            _originalItems = new ObservableCollection<InventoryItem>(_inventoryItems.Select(item =>
                new InventoryItem(item.Id, item.Name, item.Quantity, item.Price)));
        }

        /// <summary>
        /// Adds a new item to the inventory.
        /// </summary>
        /// <param name="newItem">The item to add.</param>
        public void AddItem(InventoryItem newItem)
        {
            InventoryItems.Add(newItem);
        }

        /// <summary>
        /// Updates an existing item in the inventory.
        /// </summary>
        /// <param name="updatedItem">The item with updated information.</param>
        public void UpdateItem(InventoryItem updatedItem)
        {
            var existingItem = InventoryItems.FirstOrDefault(item => item.Id == updatedItem.Id);
            if (existingItem != null)
            {
                existingItem.Name = updatedItem.Name;
                existingItem.Quantity = updatedItem.Quantity;
                existingItem.Price = updatedItem.Price;

                OnPropertyChanged(nameof(InventoryItems));
            }
        }

        /// <summary>
        /// Deletes an item from the inventory by its ID.
        /// </summary>
        /// <param name="itemId">The ID of the item to delete.</param>
        public void DeleteItem(int itemId)
        {
            var itemToRemove = InventoryItems.FirstOrDefault(item => item.Id == itemId);
            if (itemToRemove != null)
            {
                InventoryItems.Remove(itemToRemove);
            }
        }

        /// <summary>
        /// Determines whether the current user can add or delete items.
        /// </summary>
        public bool CanAddDeleteItem => IsLoggedIn;

        /// <summary>
        /// Determines whether the current user can see the 'Show User' button.
        /// </summary>
        public bool CanShowUserButton => IsAdmin;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
