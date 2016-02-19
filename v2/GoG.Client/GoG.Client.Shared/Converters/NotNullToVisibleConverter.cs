using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace GoG.Client.Converters
{
    /// <summary>
    /// Value converter that translates NOT null to <see cref="Visibility.Visible"/> and the opposite to <see cref="Visibility.Collapsed"/> 
    /// </summary>
    public sealed class NotNullToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string)
                return !String.IsNullOrWhiteSpace(value as string) ? Visibility.Visible : Visibility.Collapsed;
            else
                return (value != null) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is Visibility && (Visibility)value == Visibility.Visible);
        }
    }
}
