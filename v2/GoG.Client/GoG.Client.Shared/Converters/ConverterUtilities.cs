using System;

namespace GoG.Client.Converters
{
    /// <summary>
    /// Class containing helper methods to be used by converter classes.
    /// </summary>
    public static class ConvertersUtils
    {
        /// <summary>
        /// Matches the enum member specified by enumValue against a list contained in matchValues.
        /// If a match is found then returns true. 
        /// </summary>
        /// <param name="enumValue">
        /// A member of any enumeration type.
        /// </param>
        /// <param name="matchValues">
        /// A semicolon delimited list containing the string representation of the enumeration values that will be matched against enumValue.
        /// </param>
        /// <returns>
        /// true if a match is found between enumValue and an item in the list specified by matchValues.
        /// false otherwise.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If the enumValue is not a value of an enumeration type.
        /// If the items contained in matchValues cannot be converted to members of the enumeration type given by enumValue.
        /// </exception>
        public static bool EnumValueMatches(object enumValue, string matchValues)
        {
            if (enumValue == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(matchValues))
            {
                throw new ArgumentException("Invalid call to EnumValueMatches. The parameter matchValues cannot be null or whitespace", "matchValues");
            }

            string[] parameterArray = matchValues.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string crtValue in parameterArray)
            {
                if (crtValue.Equals(enumValue.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
