using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InventoryTracker.Utilities
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isLoggedIn)
            {
                bool showForRoleBased = parameter as string == "Visitor";
                if (showForRoleBased)
                {
                    return Visibility.Visible;
                }
                showForRoleBased = parameter as string == "NotVisitor";
                if (showForRoleBased)
                {
                    return Visibility.Collapsed;
                }

                bool showForLoggedIn = parameter as string == "IsLoggedIn";
                return showForLoggedIn ? (isLoggedIn ? Visibility.Visible : Visibility.Collapsed) : (isLoggedIn ? Visibility.Collapsed : Visibility.Visible);
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
