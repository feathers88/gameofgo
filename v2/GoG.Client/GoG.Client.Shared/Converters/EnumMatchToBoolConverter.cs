using System;
using Windows.UI.Xaml.Data;

namespace GoG.Client.Converters
{
    /// <summary>
    /// A converter that matches an enum value against a list of members of that enum. 
    /// </summary>
    public class EnumMatchToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Matches an enum value against a list of members of that enum. 
        /// If a match is found then the converter produces the value true. Otherwise it produces the value false.
        /// Example of usage: 
        ///     IsChecked="{Binding Day, Converter={StaticResource EnumMatchToBoolConverter}, ConverterParameter=Sunday;Saturday}"
        /// </summary>
        /// <param name="value">A member of any enumeration type</param>
        /// <param name="parameter">
        /// A semicolon delimited list containing the string representation of the enumeration values that will be matched against value.
        /// </param>
        /// <returns>
        /// true if a match is found between value and an item in the list specified by parameter.
        /// false otherwise.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var matches = ConvertersUtils.EnumValueMatches(value, parameter.ToString());
            return matches;
        }

        /// <summary>
        /// ConvertBack is not supported. It will throw a NotSupportedException if invoked.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
