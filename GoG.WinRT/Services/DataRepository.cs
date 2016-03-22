using FuegoLib;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Mvvm.Interfaces;

namespace GoG.WinRT.Services
{
    public class DataRepository : IDataRepository
    {
        #region Data
        readonly ISessionStateService _sessionStateService;
        #endregion Data

        #region Ctor
        public DataRepository(ISessionStateService sessionStateService)
        {
            _sessionStateService = sessionStateService;
        }
        #endregion Ctor
        
        #region Fuego Implementation

        public async Task<GoResponse> GetGameExists(Guid gameid)
        {
            try
            {
                return null;
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoResponse(GoResultCode.CommunicationError);
            }
        }

        public async Task<GoGameStateResponse> GetGameStateAsync(Guid gameid)
        {
            try
            {
                return null;
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoGameStateResponse(GoResultCode.CommunicationError, null);
            }
        }

        public async Task<GoGameStateResponse> StartAsync(Guid gameid, GoGameState state)
        {
            try
            {
                return null;
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoGameStateResponse(GoResultCode.CommunicationError, null);
            }
        }

        //public async Task<GoResponse> PlaceAsync(Guid gameid, GoGameState clientState, ObservableCollection<GoMove> moves)
        //{
        //    var c = GetFuegoClient();
        //    return await c.PlaceAsync(gameid, clientState, moves);
        //}

        public async Task<GoMoveResponse> GenMoveAsync(Guid gameid, GoColor color)
        {
            try
            {
                return null;
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoMoveResponse(GoResultCode.CommunicationError, null, null);
            }
            
        }

        public async Task<GoMoveResponse> PlayAsync(Guid gameid, GoMove move)
        {
            try
            {
                return null;
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoMoveResponse(GoResultCode.CommunicationError, null, null);
            }
        }

        public async Task<GoHintResponse> HintAsync(Guid gameid, GoColor color)
        {
            try
            {
                return null;
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoHintResponse(GoResultCode.CommunicationError, null);
            }
        }

        public async Task<GoGameStateResponse> UndoAsync(Guid gameid)
        {
            try
            {
                return null;
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoGameStateResponse(GoResultCode.CommunicationError, null);
            }
        }
        
        #endregion Fuego Implementation

        #region Private Helpers
        
        #endregion Private Helpers
    }
}
