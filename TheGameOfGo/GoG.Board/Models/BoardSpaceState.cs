using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoG.Board.Models
{
    /// <summary>
    /// Defines values that indicate the visual state of a board space.
    /// </summary>
    public enum BoardSpaceState
    {
        None,
        PlayerOne,
        PlayerTwo,
        PlayerOneHint, // not using yet
        PlayerTwoHint, // not using yet
        PlayerOneNewPiece,
        PlayerTwoNewPiece,
        PlayerOneNewCapture,
        PlayerTwoNewCapture
    }
}
