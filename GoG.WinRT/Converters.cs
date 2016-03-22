using System;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using FuegoLib;

namespace GoG.WinRT
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool) value)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class GoColorToColorConverter : IValueConverter
    {
        private static SolidColorBrush _black = new SolidColorBrush(Colors.Black);
        private static SolidColorBrush _white = new SolidColorBrush(Colors.White);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            switch (((GoColor?)value).Value)
            {
                case GoColor.Black:
                    return _black;
                case GoColor.White:
                    return _white;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    
}
