using System;
using GoG.Fuego;
using GoG.Infrastructure.Engine;
using GoG.Infrastructure.Services.Engine;
using GoG.Repository.Engine;
using GoG.ServerCommon.Logging;
using GoG.Services.Contracts;
using GoG.Services.Logging;

namespace GoG.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Fuego" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Fuego.svc or Fuego.svc.cs at the Solution Explorer and start debugging.
    public class Fuego : IFuegoService
    {
        #region Data
        private readonly ILogger _logger;
        private readonly IGoGRepository _goGRepository;
        #endregion Data

        #region Ctor

        public Fuego() : this(new Logger(), new DbGoGRepository(new Logger())) { }

        public Fuego(ILogger logger, IGoGRepository goGRepository)
        {
#if DEBUG
            logger.LogGameInfo(Guid.Empty, "Entering Fuego (service) constructor.");
#endif

            _logger = logger;
            _goGRepository = goGRepository;
            
            // Initialize the singleton fuego engine using the repository of choice.
            FuegoEngine.Init(logger, _goGRepository); // Spins up the fuego.exe instances.
        }

        #endregion Ctor

        #region IFuegoService Implementation

        public GoResponse GetGameExists(Guid gameid)
        {
            GoResponse rval;
            try
            {
                var exists = _goGRepository.GetGameExists(gameid);
                rval = new GoResponse(exists ? GoResultCode.Success : GoResultCode.GameDoesNotExist);
            }
            catch (Exception ex)
            {
                _logger.LogServerError(gameid, ex, null);
                rval = new GoGameStateResponse(GoResultCode.ServerInternalError, null);
            }
            return rval;
        }

        public GoGameStateResponse GetGameState(Guid gameid)
        {
            GoGameStateResponse rval;
            try
            {
                var state = _goGRepository.GetGameState(gameid);
                if (state == null)
                    throw new GoEngineException(GoResultCode.GameDoesNotExist, "GameId not found.");
                state.Operation = FuegoEngine.Instance.GetCurrentOperation(gameid);
                rval = new GoGameStateResponse(GoResultCode.Success, state);
            }
            catch (GoEngineException gex)
            {
                _logger.LogServerError(gameid, gex, "GameId: " + gameid);
                rval = new GoGameStateResponse(gex.Code, null);
            }
            catch (Exception ex)
            {
                _logger.LogServerError(gameid, ex, null);
                rval = new GoGameStateResponse(GoResultCode.ServerInternalError, null);
            }
            return rval;
        }

        public GoGameStateResponse Start(Guid gameid, GoGameState state)
        {
            GoGameStateResponse rval;
            try
            {
                // TODO: This code assumes only one player is AI, and uses its level setting.
                FuegoEngine.Instance.Start(gameid, state);
                rval = new GoGameStateResponse(GoResultCode.Success, state);
            }
            catch (GoEngineException gex)
            {
                _logger.LogEngineException(gameid, gex, state);
                rval = new GoGameStateResponse(gex.Code, null);
            }
            catch (Exception ex)
            {
                _logger.LogServerError(gameid, ex, state);
                rval = new GoGameStateResponse(GoResultCode.ServerInternalError, null);
            }
            return rval;
        }
        
        public GoMoveResponse GenMove(Guid gameid, GoColor color)
        {
            GoMoveResponse rval;
            try
            {
                GoMove newMove;
                GoMoveResult result;
                FuegoEngine.Instance.GenMove(gameid, color, out newMove, out result);
                rval = new GoMoveResponse(GoResultCode.Success, newMove, result);
            }
            catch (GoEngineException gex)
            {
                _logger.LogEngineException(gameid, gex, color);
                rval = new GoMoveResponse(gex.Code, null, null);
            }
            catch (Exception ex)
            {
                _logger.LogServerError(gameid, ex, color);
                rval = new GoMoveResponse(GoResultCode.ServerInternalError, null, null);
            }
            return rval;
        }

        public GoMoveResponse Play(Guid gameid, GoMove move)
        {
            GoMoveResponse rval;
            try
            {
                var result = FuegoEngine.Instance.Play(gameid, move);
                rval = new GoMoveResponse(GoResultCode.Success, move, result);
            }
            catch (GoEngineException gex)
            {
                _logger.LogEngineException(gameid, gex, move);
                rval = new GoMoveResponse(gex.Code, null, null);
            }
            catch (Exception ex)
            {
                _logger.LogServerError(gameid, ex, move);
                rval = new GoMoveResponse(GoResultCode.ServerInternalError, null, null);
            }
            return rval;
        }

        public GoHintResponse Hint(Guid gameid, GoColor color)
        {
            GoHintResponse rval;
            try
            {
                var result = FuegoEngine.Instance.Hint(gameid, color);
                rval = new GoHintResponse(GoResultCode.Success, result);
            }
            catch (GoEngineException gex)
            {
                _logger.LogEngineException(gameid, gex, "Color: " + color);
                rval = new GoHintResponse(gex.Code, null);
            }
            catch (Exception ex)
            {
                _logger.LogServerError(gameid, ex, "Color: " + color);
                rval = new GoHintResponse(GoResultCode.ServerInternalError, null);
            }
            return rval;
        }

        public GoGameStateResponse Undo(Guid gameid)
        {
            GoGameStateResponse rval;
            try
            {
                var gameState = FuegoEngine.Instance.Undo(gameid);
                rval = new GoGameStateResponse(GoResultCode.Success, gameState);
            }
            catch (GoEngineException gex)
            {
                _logger.LogEngineException(gameid, gex, "Undo move failed.");
                rval = new GoGameStateResponse(gex.Code, null);
            }
            catch (Exception ex)
            {
                _logger.LogServerError(gameid, ex, "Undo move failed.");
                rval = new GoGameStateResponse(GoResultCode.ServerInternalError, null);
            }
            return rval;
        }

        
        #endregion IFuegoService Implementation

        #region Private Helpers

        #endregion Private Helpers
    }
}
