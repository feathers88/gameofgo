using System;
using System.Text;

namespace GoG.Client
{
    public static class EngineHelpers
    {
        /// <summary>
        /// Converts a Go position like A15 to a board point, which is indexed at 0 and the Y axis is inverted.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static void EncodePosition(string p, int edgeSize, out int x, out int y)
        {
            if (p == null) throw new ArgumentNullException("p");

            try
            {
                var firstCharArray = Encoding.UTF8.GetBytes(new[] { p[0] });
                x = firstCharArray[0] - 64;
                // I is skipped.
                if (x > 8)
                    x--;
                var y2 = p.Substring(1);
                y = edgeSize - Convert.ToInt32(y2);
            }
            catch (Exception)
            {
                throw new Exception("Parameter p is not a valid Go coordinate.  Value was: " + p);
            }
        }

        /// <summary>
        /// Inverts the Y axis and moves to index base A and 1 instead of 0,0.
        /// </summary>
        /// <returns></returns>
        public static string DecodePosition(int x, int y, int edgeSize)
        {
            string rval = GetColumnLetter(x) + (edgeSize - y);
            return rval;
        }

        public static string GetColumnLetter(int x)
        {
            // I is skipped.
            if (x >= 8)
                x++;

            char[] charsOf = Encoding.UTF8.GetChars(new[] { (byte)(x + 65) });
            string rval = charsOf[0].ToString();

            return rval;
        }
    }
}
