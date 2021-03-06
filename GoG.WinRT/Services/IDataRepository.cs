using System;
using System.Threading.Tasks;
using GoG.Infrastructure.Engine;
using GoG.Infrastructure.Services.Engine;


namespace GoG.WinRT.Services
{
    public interface IDataRepository
    {
        #region Fuego
        Task<GoResponse> GetGameExists(Guid gameid);
        Task<GoGameStateResponse> GetGameStateAsync(Guid gameid);
        Task<GoGameStateResponse> StartAsync(Guid gameid, GoGameState state);
        Task<GoMoveResponse> PlayAsync(Guid gameid, GoMove move);
        Task<GoHintResponse> HintAsync(Guid gameid, GoColor color);
        Task<GoMoveResponse> GenMoveAsync(Guid gameid, GoColor color);
        Task<GoGameStateResponse> UndoAsync(Guid gameid);
        #endregion Fuego
    }
}
