using FuegoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoG.Board.Interface
{
    public interface IGenerateMoves
    {
        event EventHandler<GoMoveResultEventArgs> GoMoveChangedEvent;
    }


    public class GoMoveResultEventArgs : EventArgs
    {
        public GoMoveResult MoveResult { get; set; }
    }
}
