using System.Runtime.Serialization;
using GoG.Infrastructure.Engine;

namespace GoG.Infrastructure.Services.Engine
{
    /// <summary>
    /// Sent back to client after a move is made by the client or the server.
    /// </summary>
    public class GoMoveResponse : GoResponse
    {
        public GoMoveResponse()
        {
        }

        public GoMoveResponse(GoResultCode resultCode, GoMove move, GoMoveResult result)
            : base(resultCode)
        {
            Move = move;
            MoveResult = result;
        }

        public GoMoveResult MoveResult { get; set; }
        public GoMove Move { get; set; }
    }
}