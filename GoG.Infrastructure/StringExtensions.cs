using System.Collections.Generic;

namespace GoG.Infrastructure
{
    using System.Text;

    public static class StringExtensions
    {

        /// <summary>
        /// Takes a string[], returns a single, space separated string.  Used for taking an array
        /// of stone positions e.g. { "Q1", "F13", "B9" } and changing it into the database
        /// representation which is a simple space-separated string like: "Q1 F13 B9". 
        /// </summary>
        public static string CombineStrings(this IEnumerable<string> array)
        {
            var rval = new StringBuilder(1500);
            foreach (var s in array)
            {
                rval.Append(s);
                rval.Append(' ');
            }
            return rval.ToString().TrimEnd();
        }
    }
}
