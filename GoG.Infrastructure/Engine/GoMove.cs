namespace GoG.Infrastructure.Engine
{
    /// <summary>
    /// Represents a move.
    /// </summary>
    public class GoMove
    {
        public GoMove() { }

        public GoMove(MoveType moveType, GoColor color, string position)
        {
            MoveType = moveType;
            Color = color;
            Position = position;
        }

        /// <summary>
        /// The side that made the move.
        /// </summary>
        public GoColor Color { get; set; }

        /// <summary>
        /// Indicates the position of the piece on the board.  In the format "Q15" which means column Q, row 15, counting
        /// from the bottom left of the board.  It's 1-19, not 0-18.
        /// </summary>
        public string Position { get; set; }

        public MoveType MoveType { get; set; }

        public override string ToString()
        {
            if (MoveType == MoveType.Normal)
                return Position;
            return MoveType.ToString();
        }
    }
}