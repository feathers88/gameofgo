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

        //public GoResponse Place(Guid gameid, GoGameState clientState, GoMove[] moves)
        //{
        //    throw new NotImplementedException();
        //}

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

        public GoSaveSVGResponse SaveSGF(Guid gameid)
        {
            GoSaveSVGResponse rval;
            try
            {
                var svg = FuegoEngine.Instance.SaveSGF(gameid);
                rval = new GoSaveSVGResponse(GoResultCode.Success, svg);
            }
            catch (GoEngineException gex)
            {
                _logger.LogEngineException(gameid, gex, "Save SGF failed.");
                rval = new GoSaveSVGResponse(gex.Code, null);
            }
            catch (Exception ex)
            {
                _logger.LogServerError(gameid, ex, "Save SGF failed.");
                rval = new GoSaveSVGResponse(GoResultCode.ServerInternalError, null);
            }
            return rval;
        }

        public GoResponse LoadSGF(Guid gameid, string sgf)
        {
            GoResponse rval;
            try
            {
                FuegoEngine.Instance.LoadSGF(gameid, sgf);
                rval = new GoResponse(GoResultCode.Success);
            }
            catch (GoEngineException gex)
            {
                _logger.LogEngineException(gameid, gex, "Load SGF failed.");
                rval = new GoResponse(gex.Code);
            }
            catch (Exception ex)
            {
                _logger.LogServerError(gameid, ex, "Load SGF failed.");
                rval = new GoResponse(GoResultCode.ServerInternalError);
            }
            return rval;
        }

        //public GoResponse Undo(Guid gameid, GoGameState clientState)
        //{
        //    throw new NotImplementedException();
        //}

        //public GoResponse Quit(Guid gameid)
        //{
        //    throw new NotImplementedException();
        //}

        //public string Name()
        //{
        //    throw new NotImplementedException();
        //}

        //public string Version()
        //{
        //    throw new NotImplementedException();
        //}

        //public GoScoreResponse Score(Guid gameid, GoGameState clientState)
        //{
        //    throw new NotImplementedException();
        //}

        //public GoPositionsResponse Dead(Guid gameid, GoGameState clientState)
        //{
        //    throw new NotImplementedException();
        //}

        //public GoPositionsResponse Territory(Guid gameid, GoGameState clientState, GoColor color)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion IFuegoService Implementation

        #region Private Helpers

        #endregion Private Helpers

        //#region Data
        //private static ConcurrentDictionary<Guid, byte> _requests = new ConcurrentDictionary<Guid, byte>(100, 10000);
        //private static ConcurrentDictionary<Guid, IAIGoEngine> _games = new ConcurrentDictionary<Guid, IAIGoEngine>();
        //#endregion Data

        //#region IFuegoService Implementation

        //public GoGameStateResponse GetGameState(Guid gameid)
        //{
        //    GoGameStateResponse rval;
        //    try
        //    {
        //        VerifySingleOperationPerGame(gameid);

        //        // The clientState parameter is passed as null, so the game better exist or 
        //        // we'll get an exception (can't give state for a game that doesn't exist).
        //        var engine = VerifyEngineState(gameid, null);

        //        var state = engine.GetState();
        //        rval = new GoGameStateResponse(GoResultCode.Success, state);
        //    }
        //    catch (GoEngineException ex)
        //    {
        //        return new GoGameStateResponse(ex.Code, null);
        //    }
        //    catch (Exception)
        //    {
        //        return new GoGameStateResponse(GoResultCode.ServerInternalError, null);
        //    }
        //    finally
        //    {
        //        ReleaseRequest(gameid);
        //    }

        //    return rval;
        //}

        //public GoResponse Start(Guid gameid, GoGameState clientState)
        //{
        //    try
        //    {
        //        VerifySingleOperationPerGame(gameid);

        //        IAIGoEngine engine;
        //        if (_games.ContainsKey(gameid))
        //        {
        //            // continue game using same engine.
        //            engine = _games[gameid];
        //        }
        //        else
        //        {
        //            // create a new engine and register it
        //            engine = new FuegoEngine();
        //            _games.TryAdd(gameid, engine);
        //        }
        //        // Force a restart because we're starting a new game anyway.
        //        engine.Restart(clientState);

        //        // Save engine state to Database.
        //        SaveEngineStateToDatabase(gameid, engine);
        //    }
        //    catch (GoEngineException ex)
        //    {
        //        return new GoResponse(ex.Code);
        //    }
        //    catch (Exception ex2)
        //    {
        //        return new GoResponse(GoResultCode.ServerInternalError);
        //    }
        //    finally
        //    {
        //        ReleaseRequest(gameid);
        //    }

        //    return new GoResponse(GoResultCode.Success);
        //}

        //public GoResponse Place(Guid gameid, GoGameState clientState, GoMove[] moves)
        //{
        //    throw new NotImplementedException();
        //}

        //public GoMoveResponse GenMove(Guid gameid, GoGameState clientState, GoColor color)
        //{
        //    throw new NotImplementedException();
        //}

        //public GoMoveResponse Play(Guid gameid, GoGameState clientState, GoMove move)
        //{
        //    throw new NotImplementedException();
        //}

        //public GoResponse Undo(Guid gameid, GoGameState clientState)
        //{
        //    throw new NotImplementedException();
        //}

        //public GoResponse Quit(Guid gameid)
        //{
        //    throw new NotImplementedException();
        //}

        //public string Name()
        //{
        //    throw new NotImplementedException();
        //}

        //public string Version()
        //{
        //    throw new NotImplementedException();
        //}

        //public GoScoreResponse Score(Guid gameid, GoGameState clientState)
        //{
        //    throw new NotImplementedException();
        //}

        //public GoPositionsResponse Dead(Guid gameid, GoGameState clientState)
        //{
        //    throw new NotImplementedException();
        //}

        //public GoPositionsResponse Territory(Guid gameid, GoGameState clientState, GoColor color)
        //{
        //    throw new NotImplementedException();
        //}

        //public GoPositionsResponse Ideas(Guid gameid, GoGameState clientState, GoColor? color)
        //{
        //    throw new NotImplementedException();
        //}

        //#endregion IFuegoService Implementation

        //#region Private Helpers

        //// Use to make sure only one service operation can happen on a particular game at a time.
        //private static void VerifySingleOperationPerGame(Guid gameid)
        //{
        //    lock (_requests)
        //    {
        //        // If a request is already happening for this same gameid, abort this one.  Our
        //        // game can't be in two states simultaneously.
        //        if (_requests.ContainsKey(gameid))
        //            throw new GoEngineException(GoResultCode.SimultaneousGameMovesDetected, "Simultanous moves detected for same game.");

        //        // Save so we can prevent other requests from happening simultaneously
        //        // on this game.
        //        _requests.AddOrUpdate(gameid, 0, (g, s) => s);
        //    }
        //}

        //private static void ReleaseRequest(Guid gameid)
        //{
        //    try
        //    {
        //        lock (_requests)
        //        {
        //            if (_requests.ContainsKey(gameid))
        //            {
        //                byte val;
        //                while (!_requests.TryRemove(gameid, out val))
        //                    Thread.Sleep(5);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        ///// <summary>
        ///// Verifies the engine is running properly, and restarts it from database if necessary.
        ///// Also verifies engine state matches client state.
        ///// </summary>
        //private IAIGoEngine VerifyEngineState(Guid gameid, GoGameState clientState)
        //{
        //    try
        //    {
        //        if (_games.ContainsKey(gameid))
        //        {
        //            // continue game using same engine.
        //            var engine = _games[gameid];
                    
        //            // Make sure the game is running, possibly by restarting it and restoring
        //            // state from database.
        //            if (!engine.IsFunctioning())
        //            {
        //                var ctx = new GoGEntities();
        //                var dbgame = ctx.Games.FirstOrDefault(g => g.GameId == gameid);
        //                if (dbgame != null)
        //                {
        //                    var state = new GoGameState(dbgame.Size, dbgame.Komi, dbgame.Difficulty,
        //                                                dbgame.BlacksTurn ? GoColor.Black : GoColor.White,
        //                                                dbgame.BlackStones.Split(' '), dbgame.WhiteStones.Split(' '));
        //                    engine.Restart(state);
        //                }
        //            }

        //            // Verify the engine state matches the client.
        //            if (clientState != null && !engine.CompareEngineState(clientState))
        //            {
        //                // State mismatch - client is in the wrong and must call GetGameState().
        //                throw new GoEngineException(GoResultCode.ClientOutOfSync, "Client state mismatch.");
        //            }

        //            return engine;
        //        }
        //        else
        //        {
        //            // Either the GameId doesn't exist yet, or the service crashed or was rebooted
        //            // at some point.  We check the database for that GameId, and if it exists
        //            // we restart it.
        //            var engine = new FuegoEngine();
        //            _games.TryAdd(gameid, engine);

        //            GoGameState state;
        //            var ctx = new GoGEntities();
        //            var dbgame = ctx.Games.FirstOrDefault(g => g.GameId == gameid);
        //            // If game exists in database, restore using it.  Otherwise, must be a new
        //            // game so use the client state.
        //            if (dbgame != null)
        //            {
        //                state = new GoGameState(dbgame.Size, dbgame.Komi, 0, dbgame.BlacksTurn ? GoColor.Black : GoColor.White,
        //                                        dbgame.BlackStones.Split(' '), dbgame.WhiteStones.Split(' '));
        //                // Verify the engine state matches the client.
        //                if (clientState != null && !engine.CompareEngineState(clientState))
        //                {
        //                    // State mismatch - client is in the wrong and must call GetGameState().
        //                    throw new GoEngineException(GoResultCode.ClientOutOfSync, "Client state mismatch.");
        //                }
        //            }
        //            else
        //            {
        //                // No such game exists.  If clientState is null, that means user is asking
        //                // for state, which we can't give them, so we throw an exception.
        //                if (clientState == null)
        //                    throw new GoEngineException(GoResultCode.GameDoesNotExist, "No such game.");

        //                // New game - use client state.
        //                state = clientState;
        //            }

        //            engine.Start(state);

        //            return engine;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        

        //#endregion Private Helpers

    }
}
