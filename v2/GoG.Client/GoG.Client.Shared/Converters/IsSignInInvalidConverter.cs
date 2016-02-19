using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace GoG.Client.Converters
{
    /// <summary>
    /// Value converter that translates a boolean value to an invalid sign-in message.
    /// </summary>
    public sealed class IsSignInInvalidConverter : IValueConverter
    {
        ResourceLoader resourceLoader = new ResourceLoader();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? resourceLoader.GetString("ErrorInvalidSignInErrorMessage") : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
