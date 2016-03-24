using FuegoLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using GoG.Infrastructure.Engine;
using GoG.Infrastructure.Services.Engine;

namespace GoG.WinRT.Services
{
    public class DataRepository : IDataRepository
    {
        #region Data
        readonly ISessionStateService _sessionStateService;
        private FuegoInstance _fuego;
        private GoOperation _currentOperation = GoOperation.Starting;
        // If the State is null, means Start() has not been called, which could be because this FuegoInstance 
        // has never been used, or once belonged to another gameid and was repurposed.
        private GoGameState _state;
        readonly List<string> _debugLines = new List<string>();
        private readonly ConcurrentQueue<string> _inputs = new ConcurrentQueue<string>();
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
            GoGameStateResponse rval;
            try
            {
                _currentOperation = GoOperation.Starting;
                _fuego = new FuegoInstance();
                _fuego.StartGame(state.Size);

                var level = state.Player1.PlayerType == PlayerType.AI ? state.Player1.Level : state.Player2.Level;
                _state = state;

                // Set up parameters and clear board.
                //await WriteCommand("uct_max_memory", (1024*1024*250).ToString());
                //await ReadResponse();
                await WriteCommand("boardsize", state.Size.ToString(CultureInfo.InvariantCulture));
                await ReadResponse();

                if (level < 3)
                {
                    await WriteCommand("uct_param_player max_games", ((level + 1) * 10).ToString(CultureInfo.InvariantCulture));
                    await ReadResponse();
                }
                else if (level < 6)
                {
                    await WriteCommand("uct_param_player max_games", (level * 2000).ToString(CultureInfo.InvariantCulture));
                    await ReadResponse();
                }
                else if (level < 9)
                {
                    await WriteCommand("uct_param_player max_games", (level * 10000).ToString(CultureInfo.InvariantCulture));
                    await ReadResponse();
                }
                else //if (level < 9)
                {
                    await WriteCommand("uct_param_player max_games", Int32.MaxValue.ToString(CultureInfo.InvariantCulture));
                    await ReadResponse();
                }
                //WriteCommand("komi", state.Komi.ToString(CultureInfo.InvariantCulture));
                //ReadResponse();
                await WriteCommand("clear_board");
                await ReadResponse();

                await WriteCommand("go_param_rules", "capture_dead 1");
                await ReadResponse();

                await WriteCommand("showboard");
                await ReadResponse();
                
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

                        await WriteCommand("play", (m.Move.Color == GoColor.Black ? "black" : "white") + ' ' + position);
                        await ReadResponse();
                    }
                }



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
                _currentOperation = GoOperation.Idle;
            }

            return rval;
        }

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

        private async Task WriteCommand(string cmd, string value = null)
        {
#if DEBUG
            Debug.WriteLine("WRITING: ");
            Debug.WriteLine(cmd);
#endif
            _fuego.Write(cmd);
            if (value != null)
            {
                Debug.WriteLine(" ");
                _fuego.Write(" ");
                Debug.WriteLine(value);
                _fuego.Write(value);
            }
#if DEBUG
            Debug.WriteLine("\n\n");
#endif
            _fuego.Write("\n\n");
            //_fuego.Flush();
            await Task.Delay(10);
        }

        class MyResponse
        {
            public string Code { get; set; }
            public string Msg { get; set; }
        }

        /// <summary>
        /// Puts the returned id and message into variables _id and _message.  Throws a 
        /// GoEnginException if Fuego complains.
        /// </summary>
        private async Task<MyResponse> ReadResponse()
        {
            var rval = new MyResponse();
            
            _debugLines.Clear();

            var haveResult = false;
            while (true)
            {
                await Task.Delay(50); // allow more text to come out

                ReadMoreLines();

                while (!_inputs.IsEmpty)
                {
                    string line;
                    _inputs.TryDequeue(out line);
#if DEBUG
                    Debug.WriteLine("Read: " + (line ?? "(NULL)"));
#endif

                    // If empty line, eats it, otherwise parses the line.
                    if (!String.IsNullOrEmpty(line))
                    {
                        switch (line[0])
                        {
                            case '?':
                                // If line starts with '?', indicates an error has occurred in Fuego.
                                haveResult = true;
                                rval = await ParseEngineOutput(line);
                                await ParseErrorAndThrowException(rval.Code, rval.Msg);
                                break;
                            case '=':
                                // If line starts with '=', no error.
                                haveResult = true;
                                rval = await ParseEngineOutput(line);
                                break;
                            default:
                                // If line starts with something else, save it.
                                _debugLines.Add(line);
                                break;
                        }
                    }
                }

                // Delay a bit to ensure we have the full response.  The result line (starts with
                // = or ?) can come first, and we put all other lines in _debugLines.
                await Task.Delay(50);
                ReadMoreLines();
                if (_inputs.IsEmpty && haveResult)
                    break;
            }

            return rval;
        }

        private void ReadMoreLines()
        {
            while (true)
            {
                var newline = _fuego.ReadLine();
                if (!String.IsNullOrEmpty(newline))
                    _inputs.Enqueue(newline);
                else
                    break;
            }
        }

        private async Task ParseErrorAndThrowException(string errorId, string errorMessage)
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

            await WriteCommand("showboard");
            await ReadResponse();
            var msg = new StringBuilder();
            foreach (var l in _debugLines)
                msg.AppendLine(l);
            if (code == null)
            {
                msg.Append("Error " + errorId + ": " + errorMessage);
                code = GoResultCode.OtherEngineError;
            }
            else
                msg.Append(errorMessage);

            throw new GoEngineException(code.Value, msg.ToString());
        }


        // Parses everything after the first character on a response line.
        private async Task<MyResponse> ParseEngineOutput(string it)
        {
            var rval = new MyResponse();

            if (it[1] == ' ')
            {
                // code is not present
                rval.Code = null;
                rval.Msg = it.Substring(2);
            }
            else
            {
                // code is present
                var strpos = it.IndexOf(' ', 2);
                rval.Code = it.Substring(1, strpos - 1);
                rval.Msg = it.Substring(strpos + 1);
            }

            return rval;
        }

        #endregion Private Helpers
    }
}
