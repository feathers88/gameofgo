using FuegoLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using GoG.Infrastructure;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using GoG.Infrastructure.Engine;
using GoG.Infrastructure.Services.Engine;
using Newtonsoft.Json;

namespace GoG.WinRT.Services
{
    public class DataRepository : IDataRepository
    {
        #region Data

        const string StateFilename = "state.json";

        readonly ISessionStateService _sessionStateService;

        // _fuego is the AI component, written in C++.  It uses threads, but it also
        // hangs, so every time we call methods on it we must create a thread.
        private FuegoInstance _fuego;

        //private GoOperation _currentOperation = GoOperation.Starting;

        // If the State is null, means Start() has not been called, which could be because this FuegoInstance 
        // has never been used, or once belonged to another gameid and was repurposed.
        private GoGameState _state;

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
                var matches = gameid != Guid.Empty && _fuego.Guid == gameid;
                return new GoResponse(matches ? GoResultCode.Success : GoResultCode.GameDoesNotExist);
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
                if (_fuego == null || _fuego.Guid == Guid.Empty || _fuego.Guid != gameid)
                    return new GoGameStateResponse(GoResultCode.GameDoesNotExist, null);

                if (_state == null)
                    await LoadState();

                return new GoGameStateResponse(GoResultCode.Success, _state);
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoGameStateResponse(GoResultCode.CommunicationError, null);
            }
        }

        /// <summary>
        /// Starts the fuego engine and resets its game state to the given state.
        /// </summary>
        /// <param name="gameid"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<GoGameStateResponse> StartAsync(Guid gameid, GoGameState state)
        {
            GoGameStateResponse rval;
            try
            {
                state.Operation = GoOperation.Starting;

                await Task.Factory.StartNew(
                    () =>
                    {
                        _fuego = new FuegoInstance();
                        _fuego.StartGame(state.Size, gameid);

                        var level = state.Player1.PlayerType == PlayerType.AI ? state.Player1.Level : state.Player2.Level;
                        
                        // Set up parameters and clear board.
                        //await WriteCommand("uct_max_memory", (1024 * 1024 * 250).ToString());
                        
                        if (level < 3)
                        {
                            ParseResponse(WriteCommand("uct_param_player max_games", ((level + 1) * 10).ToString(CultureInfo.InvariantCulture)));
                        }
                        else if (level < 6)
                        {
                            ParseResponse(WriteCommand("uct_param_player max_games", (level * 2000).ToString(CultureInfo.InvariantCulture)));
                        }
                        else if (level < 9)
                        {
                            ParseResponse(WriteCommand("uct_param_player max_games", (level * 10000).ToString(CultureInfo.InvariantCulture)));
                        }
                        else //if (level < 9)
                        {
                            ParseResponse(WriteCommand("uct_param_player max_games", int.MaxValue.ToString(CultureInfo.InvariantCulture)));
                        }

                        //WriteCommand("komi", state.Komi.ToString(CultureInfo.InvariantCulture));
                        //ReadResponse();
                        ParseResponse(WriteCommand("clear_board"));
                        ParseResponse(WriteCommand("go_param_rules", "capture_dead 1"));

                        // Set up board with some pre-existing moves.
                        if (state.GoMoveHistory != null && state.GoMoveHistory.Count > 0)
                        {
                            // Must actually play every move back because otherwise undo operations
                            // won't work.
                            foreach (var m in state.GoMoveHistory)
                            {
                                string position;
                                switch (m.Move.MoveType)
                                {
                                    case MoveType.Normal:
                                        position = m.Move.Position;
                                        break;
                                    case MoveType.Pass:
                                        position = "PASS";
                                        break;
                                    default:
                                        throw new ArgumentException("Unrecognized move type: " + m.Move.MoveType);
                                }

                                ParseResponse(WriteCommand("play", (m.Move.Color == GoColor.Black ? "black" : "white") + ' ' + position));
                            }
                        }

                        _state = state;
                        SaveState().Wait();
                    });

                rval = new GoGameStateResponse(GoResultCode.Success, state);
            }
            catch (GoEngineException gex)
            {
                rval = new GoGameStateResponse(gex.Code, null);
            }
            catch (Exception ex)
            {
                rval = new GoGameStateResponse(GoResultCode.ServerInternalError, null);
            }
            finally
            {
                state.Operation = GoOperation.Idle;
            }

            return rval;
        }

        public async Task<GoMoveResponse> GenMoveAsync(Guid gameid, GoColor color)
        {
            GoMoveResponse rval = null;

            _state.Operation = GoOperation.GenMove;

            try
            {
                await Task.Factory.StartNew(
                        () =>
                        {
                            // This debug code generates a resign from the AI randomly.
                            //int x = r.Next(5);
                            //if (x == 0)
                            //{
                            //    newMove = new GoMove(MoveType.Resign, color, null);
                            //    //WriteCommand("play", color == GoColor.Black ? "black resign" : "white resign");
                            //    //ReadResponse();
                            //    result = AddMoveAndUpdateStateAndSaveToDatabase(newMove);
                            //    return;
                            //}

                            var result = ParseResponse(WriteCommand("genmove", color == GoColor.Black ? "black" : "white"));

                            GoMove newMove;
                            switch (result.Msg)
                            {
                                case "PASS":
                                    newMove = new GoMove(MoveType.Pass, color, null);
                                    break;
                                case "resign":
                                    newMove = new GoMove(MoveType.Resign, color, null);
                                    break;
                                default:
                                    newMove = new GoMove(MoveType.Normal, color, result.Msg);
                                    break;
                            }

                            // Add to move history and record new game state in database so user can 
                            // see what happened.
                            var moveResult = AddMoveAndUpdateStateAndSaveState(newMove);

                            rval = new GoMoveResponse(GoResultCode.Success, newMove, moveResult);
                        });
            }
            catch (GoEngineException gex)
            {
                rval = new GoMoveResponse(gex.Code, null, null);
            }
            catch (Exception ex)
            {
                rval = new GoMoveResponse(GoResultCode.ServerInternalError, null, null);
            }
            finally
            {
                _state.Operation = GoOperation.Idle;
            }

            Debug.Assert(rval != null, "rval != null");

            return rval;
        }

        public async Task<GoMoveResponse> PlayAsync(Guid gameid, GoMove move)
        {
            GoMoveResponse rval;

            GoMoveResult moveResult = null;
            try
            {
                if (move.MoveType == MoveType.Resign)
                {
                    // Do nothing - fuego doesn't support the command to resign.
                }
                else
                {
                    string position;
                    switch (move.MoveType)
                    {
                        case MoveType.Normal:
                            position = move.Position;
                            break;
                        case MoveType.Pass:
                            position = "PASS";
                            break;
                        default:
                            throw new ArgumentException("Unrecognized move type: " + move.MoveType);
                    }

                    await Task.Factory.StartNew(
                        () =>
                        {
                            // This throws a GoEngineException on any failure.
                            ParseResponse(WriteCommand("play", (move.Color == GoColor.Black ? "black" : "white") + ' ' + position));

                            // Add to move history and persist new game state so user can 
                            // see what happened.
                            moveResult = AddMoveAndUpdateStateAndSaveState(move);
                        });
                }

                Debug.Assert(moveResult != null, "moveResult != null");
                rval = new GoMoveResponse(GoResultCode.Success, move, moveResult);
            }
            catch (GoEngineException gex)
            {
                rval = new GoMoveResponse(gex.Code, null, null);
            }
            catch (Exception ex)
            {
                rval = new GoMoveResponse(GoResultCode.ServerInternalError, null, null);
            }
            //finally
            //{
            //    await SaveState();
            //}

            return rval;
        }

        public async Task<GoHintResponse> HintAsync(Guid gameid, GoColor color)
        {
            try
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoGameStateResponse(GoResultCode.CommunicationError, null);
            }
        }

        #endregion Fuego Implementation

        #region Private Helpers

        private string WriteCommand(string cmd, string value = null)
        {
            var s = cmd;
            if (value != null)
                s += ' ' + value + "\n\n";
#if DEBUG
            Debug.Write("WRITING COMMAND: " + s);
#endif
            var result = _fuego.HandleCommand(s);
            return result;
        }

        private class MyResponse
        {
            public MyResponse()
            {
                
            }

            public MyResponse(string code, IEnumerable<string> lines)
            {
                Code = code;
                Lines = new List<string>(lines);
            }

            public MyResponse(string code, string msg)
            {
                Code = code;
                Lines = new List<string> {msg};
            }

            public string Code { get; set; }
            public List<string> Lines { get; }
            public string Msg => Lines?[0];
        }

        /// <summary>
        /// Puts the returned id and message into variables _id and _message.  Throws a 
        /// GoEnginException if Fuego complains.
        /// </summary>
        private MyResponse ParseResponse(string str)
        {
            var rval = new MyResponse();

            foreach (var line in str.Split('\n'))
            {
#if DEBUG
                Debug.WriteLine("Read: " + (line ?? "(NULL)"));
#endif

                // If empty line, eats it, otherwise parses the line.
                if (!string.IsNullOrEmpty(line))
                {
                    switch (line[0])
                    {
                        case '?':
                            // If line starts with '?', indicates an error has occurred in Fuego.
                            rval = ParseEngineOutput(line);
                            ParseErrorAndThrowException(rval.Code, rval.Msg);
                            break;
                        case '=':
                            // If line starts with '=', no error.
                            rval = ParseEngineOutput(line);
                            break;
                        default:
                            // If line starts with something else, save it.
                            rval.Lines.Add(line);
                            break;
                    }
                }
            }

            return rval;
        }

        private void ParseErrorAndThrowException(string errorId, string errorMessage)
        {
            GoResultCode? code = null;

            if (errorMessage.StartsWith("illegal move:"))
            {
                if (errorMessage.EndsWith("(occupied)"))
                    code = GoResultCode.IllegalMoveSpaceOccupied;
                else if (errorMessage.EndsWith("(suicide)"))
                    code = GoResultCode.IllegalMoveSuicide;

                else if (errorMessage.EndsWith("(superko)"))
                    code = GoResultCode.IllegalMoveSuperKo;
                else
                    code = GoResultCode.OtherIllegalMove;
            }

            if (errorMessage == "cannot score")
                code = GoResultCode.CannotScore;

            var result = ParseResponse(WriteCommand("showboard"));
            
            if (code == null)
            {
                result.Lines.Add("Error " + errorId + ": " + errorMessage);
                code = GoResultCode.OtherEngineError;
            }
            else
                result.Lines.Add(errorMessage);

            throw new GoEngineException(code.Value, result.Lines.CombineStrings("\n"));
        }

        // Parses everything after the first character on a response line.
        private MyResponse ParseEngineOutput(string it)
        {
            if (it[1] == ' ')
            {
                // code is not present
                return new MyResponse(null, it.Substring(2));
            }
            else
            {
                // code is present
                var strpos = it.IndexOf(' ', 2);
                return new MyResponse(it.Substring(1, strpos - 1), it.Substring(strpos + 1));
            }
        }

        private async Task SaveState()
        {
            try
            {
                var str = JsonConvert.SerializeObject(_state);

                var storageFolder = ApplicationData.Current.LocalFolder;
                var file = await storageFolder.CreateFileAsync(StateFilename, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, str);
            }
            catch (Exception ex)
            {
                
                throw;
            }
            
        }

        private async Task LoadState()
        {
            try
            {
                var storageFolder = ApplicationData.Current.LocalFolder;
                var file = await storageFolder.GetFileAsync(StateFilename);
                var str = await FileIO.ReadTextAsync(file);

                _state = JsonConvert.DeserializeObject<GoGameState>(str);
            }
            catch (Exception ex)
            {
                
                throw;
            }
            
        }

        private GoMoveResult AddMoveAndUpdateStateAndSaveState(GoMove move)
        {
            GoMoveResult rval;

            try
            {
                var beforeBlack = _state.BlackPositions;
                var beforeWhite = _state.WhitePositions;

                GetStones(); // Gets the new _state.BlackPositions and _state.WhitePositions.

                if (move.Color == GoColor.Black)
                    rval = new GoMoveResult(beforeWhite, _state.WhitePositions);
                else
                    rval = new GoMoveResult(beforeBlack, _state.BlackPositions);

                _state.GoMoveHistory.Add(new GoMoveHistoryItem { Move = move, Result = rval });

                // Change turn.
                _state.WhoseTurn = _state.WhoseTurn == GoColor.Black ? GoColor.White : GoColor.Black;

                switch (move.MoveType)
                {
                    case MoveType.Resign:
                        _state.Status = move.Color == GoColor.Black
                            ? GoGameStatus.WhiteWonDueToResignation
                            : GoGameStatus.BlackWonDueToResignation;
                        //var gameResult = CalculateGameResult();
                        //State.WinMargin = Decimal.Parse(gameResult.Substring(1));
                        break;
                    case MoveType.Pass:
                        // If previous move was a pass also, calculate winner.
                        var moveCount = _state.GoMoveHistory.Count;
                        bool previousMoveWasPass = moveCount >= 2 &&
                                                    _state.GoMoveHistory[moveCount - 2].Move.MoveType == MoveType.Pass;
                        if (previousMoveWasPass)
                        {
                            var gameResult2 = CalculateGameResult();
                            _state.WinMargin = decimal.Parse(gameResult2.Substring(1));
                            _state.Status = gameResult2.StartsWith("B") ? GoGameStatus.BlackWon : GoGameStatus.WhiteWon;
                        }
                        break;
                }

                rval.Status = _state.Status;
                rval.WinMargin = _state.WinMargin;

                // Save to database.  Note we can't await this because it produces a strange
                // runtime error, seems to be an issue in WinRT or the C# compiler somewhere.
                // Wait() works fine if the caller is already on its own thread.
                SaveState().Wait();
            }
            catch (Exception ex)
            {
                throw;
            }
            
            return rval;
        }

        private async Task UpdateStateFromExeAndSaveState()
        {
#if DEBUG
            var result = ParseResponse(WriteCommand("showboard"));
#endif

            GetStones(); // Gets the new _state.BlackPositions and _state.WhitePositions.

            // Determine whose turn.
            _state.WhoseTurn = _state.WhoseTurn == GoColor.Black ? GoColor.White : GoColor.Black;

            // Save to database.
            await SaveState();
        }

        /// <summary>
        /// Call after each move to save state.
        /// </summary>
        private void GetStones()
        {
            var result = ParseResponse(WriteCommand("list_stones", "black"));
            _state.BlackPositions = result.Msg;

            result = ParseResponse(WriteCommand("list_stones", "white  "));
            _state.WhitePositions = result.Msg;
        }

        private string CalculateGameResult()
        {
            var result = ParseResponse(WriteCommand("final_score", null));

            return result.Msg;
        }

        #endregion Private Helpers
    }
}
