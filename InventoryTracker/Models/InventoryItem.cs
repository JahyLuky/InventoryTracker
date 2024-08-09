using System.ComponentModel;

namespace InventoryTracker.Models
{
    /// <summary>
    /// Represents an inventory item with properties such as Id, Name, Quantity, and Price.
    /// Implements INotifyPropertyChanged to support data binding in WPF applications.
    /// </summary>
    public class InventoryItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Event raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private int _id;

        /// <summary>
        /// Gets or sets the unique identifier of the inventory item.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string _name;

        /// <summary>
        /// Gets or sets the name of the inventory item.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private int _quantity;

        /// <summary>
        /// Gets or sets the quantity of the inventory item.
        /// </summary>
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        private double _price;

        /// <summary>
        /// Gets or sets the price of the inventory item.
        /// </summary>
        public double Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event to notify subscribers of a property change.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
