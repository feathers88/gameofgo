using System;
using System.Collections.Generic;

namespace GoG.Infrastructure.Engine
{
    /// <summary>
    /// Game state contains information about the game and previous moves.
    /// </summary>
    public class GoGameState
    {
        public GoGameState()
        {
        }

        public GoGameState(Guid gameId, byte size,
                           GoPlayer player1, GoPlayer player2,
                           GoGameStatus status,       
                           GoColor whoseTurn, string blackPositions, string whitePositions,
                           List<GoMoveHistoryItem> goMoveHistory, decimal winMargin)
        {
            GameId = gameId;
            Size = size;
            Player1 = player1;
            Player2 = player2;
            Status = status;
            WhoseTurn = whoseTurn;
            BlackPositions = blackPositions;
            WhitePositions = whitePositions;
            GoMoveHistory = goMoveHistory;
            WinMargin = winMargin;
        }

        ///// <summary>
        ///// Does a deep comparison.
        ///// </summary>
        //public override bool Equals(object obj)
        //{
        //    var o = obj as GoGameState;
        //    if (o == null)
        //        return false;
        //    return Size == o.Size &&
        //           Komi == o.Komi &&  // float comparisons can be off by a tiny amount; this corrects that potential error
        //           Level == o.Level &&
        //           WhoseTurn == o.WhoseTurn &&
        //           AreSame(BlackPositions, o.BlackPositions) &&
        //           AreSame(WhitePositions, o.WhitePositions);
        //}

        //// Compares two position arrays, which might contain the same pieces, but in a different order.
        //private bool AreSame(string a, string b)
        //{
        //    if (a == null && b == null)
        //        return true;
        //    if (a == null || b == null)
        //        return false;
            
        //    // Everything in a should be b, and vice versa.
        //    return a.All(b.Contains) && b.All(a.Contains);
        //}

        public GoGameStatus Status { get; set; }
        public decimal WinMargin { get; set; }
        public GoPlayer Player1 { get; set; }
        public GoPlayer Player2 { get; set; }
        public GoOperation Operation { get; set; }

        public Guid GameId { get; set; }

        /// <summary>
        /// Board edge size, usually 9x9, 13x13, or 19x19.
        /// </summary>
        public byte Size { get; set; }

        ///// <summary>
        ///// Number of seconds alloted for each side to play each turn.  0 for no limit.
        ///// </summary>
        //public int SecondsPerTurn { get; set; }

        /// <summary>
        /// Whose turn is it?  Black or White?
        /// </summary>
        public GoColor WhoseTurn { get; set; }

        /// <summary>
        /// Position of all the black stones.
        /// </summary>
        public string BlackPositions { get; set; }
        
        /// <summary>
        /// Position of all the white stones.
        /// </summary>
        public string WhitePositions { get; set; }

        public List<GoMoveHistoryItem> GoMoveHistory { get; set; }
    }
}