using System;
using Windows.UI.Xaml.Data;

namespace GoG.Client.Converters
{
    public class SumConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null)
                return 0D;
            double i = System.Convert.ToDouble(value);
            double j = System.Convert.ToDouble(parameter);
            return i + j;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
