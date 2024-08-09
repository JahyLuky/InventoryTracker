using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InventoryTracker.Utilities
{
    /// <summary>
    /// Converts boolean values to Visibility enumeration values and vice versa.
    /// Optionally supports an "Inverse" mode to toggle visibility behavior.
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a Visibility enumeration value.
        /// </summary>
        /// <param name="value">The boolean value to convert.</param>
        /// <param name="targetType">The type of the target property (not used).</param>
        /// <param name="parameter">Optional parameter to indicate "Inverse" mode.</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>Visibility.Visible if true (or false in "Inverse" mode), Visibility.Collapsed otherwise.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (parameter != null && parameter.ToString() == "Inverse")
                {
                    return boolValue ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    return boolValue ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return Visibility.Collapsed; // Default to collapsed if value is not boolean
        }

        /// <summary>
        /// Converts a Visibility enumeration value back to a boolean value.
        /// </summary>
        /// <param name="value">The Visibility enumeration value to convert.</param>
        /// <param name="targetType">The type to convert to (not used).</param>
        /// <param name="parameter">Optional parameter to indicate "Inverse" mode.</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>True if Visibility.Visible (or false in "Inverse" mode), false otherwise.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                if (parameter != null && parameter.ToString() == "Inverse")
                {
                    return visibility == Visibility.Collapsed;
                }
                else
                {
                    return visibility == Visibility.Visible;
                }
            }
            return false; // Default to false if value is not Visibility
        }
    }
}
