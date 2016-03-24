using GoG.Infrastructure.Engine;
using System;
using System.Text;

namespace GoG.Infrastructure
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

        public static string GetResultCodeFriendlyMessage(GoResultCode code)
        {
            string msg = null;
            switch (code)
            {
                case GoResultCode.CommunicationError:
                    msg = "Communication error.  Are you connected to the Internet?";
                    break;
                case GoResultCode.EngineBusy:
                    msg = "Server is playing too many simultaneous games.  Please wait a minute and try again.";
                    break;
                case GoResultCode.OtherEngineError:
                case GoResultCode.ServerInternalError:
                    msg = "Something blew up!  Please try again.";
                    break;
                case GoResultCode.GameAlreadyExists:
                    msg = "Game already exists.  Please try again.";
                    break;
                case GoResultCode.GameDoesNotExist:
                    msg = "Your game was aborted due to inactivity.  Please start another one.";
                    break;
                case GoResultCode.ClientOutOfSync:
                    msg = "Your game was out of sync.";
                    break;
                case GoResultCode.SimultaneousRequests:
                    msg =
                        "Are you playing this game on another device right now?  If so, please leave and re-enter the game.";
                    break;
                case GoResultCode.Success:
                    msg = String.Empty;
                    break;
                case GoResultCode.IllegalMoveSpaceOccupied:
                    msg = "That space is occupied.";
                    break;
                case GoResultCode.IllegalMoveSuicide:
                    msg = "That move is suicide, which is not allowed.";
                    break;
                case GoResultCode.IllegalMoveSuperKo:
                    msg = "That move would replicate a previous board position, which violates the \"superko\" rule.";
                    break;
                case GoResultCode.OtherIllegalMove:
                    msg = "That move is not legal.";
                    break;
                case GoResultCode.CannotScore:
                    msg = "There are one or more stones that may be dead (or not).  Please continue playing until this situation is resolved.";
                    break;
                case GoResultCode.CannotSaveSGF:
                    msg = "Cannot save SGF.";
                    break;
                default:
                    throw new Exception("Unsupported value for GoResultCode: " + code);
            }
            return msg;
        }
    }
}
