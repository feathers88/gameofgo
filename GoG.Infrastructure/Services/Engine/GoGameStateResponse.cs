using GoG.Infrastructure.Engine;

namespace GoG.Infrastructure.Services.Engine
{
    /// <summary>
    /// Full game / board state to be sent to the client on request.
    /// </summary>
    public class GoGameStateResponse : GoResponse
    {
        // This empty constructor is so WCF's DataContractSerializer is able to build an instance of this type.
        public GoGameStateResponse()
        {
        }

        public GoGameStateResponse(GoResultCode resultCode, GoGameState gameState) : base(resultCode)
        {
            GameState = gameState;
        }

        public GoGameState GameState { get; set; }
    }
}