using System.Data.Entity.Migrations.Model;
using System.Globalization;
using System.Security;
using GoG.Infrastructure.Engine;
using GoG.Infrastructure.Services.Engine;
using GoG.Repository.Engine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using GoG.ServerCommon.Logging;

namespace GoG.Fuego
{
    public class FuegoInstance
    {
        #region Data
        private readonly IGoGRepository _goGRepository;
        private readonly ILogger _logger;

        // If the State is null, means Start() has not been called, which could be because this FuegoInstance 
        // has never been used, or once belonged to another gameid and was repurposed.
        public GoGameState State;

        private StreamWriter _writer;
        private readonly ConcurrentQueue<string> _inputs = new ConcurrentQueue<string>();

        private readonly long _memoryPerInstance;

        static readonly Random r = new Random();

        private SimpleFuegoGtpEngine _engine = new SimpleFuegoGtpEngine();
        #endregion Data

        #region Ctor

        public FuegoInstance(IGoGRepository goGRepository, long memoryPerInstance, ILogger logger)
        {
            _goGRepository = goGRepository;
            _memoryPerInstance = memoryPerInstance;
            _logger = logger;
            CurrentOperation = GoOperation.Idle;

            logger.LogGameInfo(Guid.Empty, "Starting a new FuegoInstance.");

            EnsureRunning();
        }

        #endregion Ctor

        #region Properties

        private GoOperation _currentOperation;
        public GoOperation CurrentOperation
        {
            get { return _currentOperation; }
            set
            {
                if (value == GoOperation.Idle)
                    LastOperationFinished = DateTimeOffset.Now;
                _currentOperation = value;
            }
        }

        public Process Process { get; set; }
        public Guid CurrentGameId { get; set; }
        public DateTimeOffset LastOperationFinished { get; set; }
        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Checks if fuego is running and if not restarts it.  If no state, starts the game and sets it up
        /// to match the database.
        /// </summary>
        public void EnsureRunning()
        {
            if (Process == null || Process.HasExited)
            {
                State = null;
                RestartProcess();
            }
            if (State == null)
            {
                // Try getting state from database.  If it doesn't exist in the database, then the user
                // never started the game (and we probably shouldn't have gotten to this point).  If it
                // does exist in the database, then we must initialize fuego.exe with the database state.
                var state = _goGRepository.GetGameState(this.CurrentGameId);
                if (state != null)
                    Start(state);
            }
        }

        public void RestartProcess()
        {
            // Kill any existing process.
            if (Process != null)
            {
                try
                {
                    Process.OutputDataReceived -= ProcessOnOutputDataReceived;
                    Process.Kill();
                    Process = null;
                }
// ReSharper disable EmptyGeneralCatchClause
                catch
// ReSharper restore EmptyGeneralCatchClause
                {
                }
            }

            // gives exe 1 second to start up, then another 2, then another 4.
            var wait = 1000; // 1 second
            var success = false;
            for (int t = 0; t < 3; t++) // try a few times to connect, then give up
            {
                try
                {
                    // Create the process.
                    Process = new Process
                    {
                        StartInfo =
                        {
                            FileName = FuegoEngine.ExePath,
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            LoadUserProfile = false
                        }
                    };

                    Process.Start();

                    Thread.Sleep(wait); // give exe a chance to start up

                    _writer = Process.StandardInput;

                    // This method is much more reliable than trying to read standard output.
                    Process.OutputDataReceived += ProcessOnOutputDataReceived;
                    Process.BeginOutputReadLine();

                    success = true;
                    break;
                }
                catch (Exception ex)
                {
                    DebugMessage("Error connecting to Fuego on attempt #" + t + ". " + ex.Message);
                    if (Process != null)
                        Process.Kill();
                    wait *= 2;
                    Thread.Sleep(wait);
                }
            }
            if (!success)
            {
                DebugMessage("Giving up on connecting to Fuego.");
                throw new GoEngineException(GoResultCode.ServerInternalError, "Couldn't connect to Fuego.");
            }
        }

        /// <summary>
        /// Initialize game parameters and board to an given state.
        /// </summary>
        public void Start(GoGameState state)
        {
            try
            {
                if (state == null) throw new ArgumentNullException("state");

                int level = state.Player1.PlayerType == PlayerType.AI ? state.Player1.Level : state.Player2.Level;
                State = state;

                // Set up parameters and clear board.
                WriteCommand("uct_max_memory", _memoryPerInstance.ToString(CultureInfo.InvariantCulture));
                ReadResponse();
                WriteCommand("boardsize", state.Size.ToString(CultureInfo.InvariantCulture));
                ReadResponse();
                if (level < 3)
                {
                    WriteCommand("uct_param_player max_games", ((level+1) * 10).ToString(CultureInfo.InvariantCulture));
                    ReadResponse();
                }
                else if (level < 6)
                {
                    WriteCommand("uct_param_player max_games", (level * 2000).ToString(CultureInfo.InvariantCulture));
                    ReadResponse();
                }
                else if (level < 9)
                {
                    WriteCommand("uct_param_player max_games", (level * 10000).ToString(CultureInfo.InvariantCulture));
                    ReadResponse();
                }
                else //if (level < 9)
                {
                    WriteCommand("uct_param_player max_games", Int32.MaxValue.ToString(CultureInfo.InvariantCulture));
                    ReadResponse();
                }
                //WriteCommand("komi", state.Komi.ToString(CultureInfo.InvariantCulture));
                //ReadResponse();
                WriteCommand("clear_board");
                ReadResponse();

                WriteCommand("go_param_rules", "capture_dead 1");
                ReadResponse();

                //WriteCommand("showboard");
                //ReadResponse();
            

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

                        WriteCommand("play", (m.Move.Color == GoColor.Black ? "black" : "white") + ' ' + position);
                        ReadResponse();
                    }

                    //var param = new StringBuilder((19 * 19) * 9);
                    //if (state.BlackPositions != null)
                    //{
                    //    foreach (var p in state.BlackPositions.Trim().Split(' '))
                    //    {
                    //        if (!String.IsNullOrEmpty(p))
                    //        {
                    //            param.Append(" black ");
                    //            param.Append(p);
                    //        }
                    //    }
                    //}
                    //if (state.WhitePositions != null)
                    //{
                    //    foreach (var p in state.WhitePositions.Trim().Split(' '))
                    //    {
                    //        if (!String.IsNullOrEmpty(p))
                    //        {
                    //            param.Append(" white ");
                    //            param.Append(p);
                    //        }
                    //    }
                    //}
                    //WriteCommand("gogui-setup", param.ToString().Trim());
                    //ReadResponse();
                }
            }
            catch (Exception)
            {
                State = null; // Forces next attempt to reload state again.
                throw;
            }
        }

        ///// <summary>
        ///// Used to setup handicap or other initial board position.  Fails if there are already pieces on the board.
        ///// </summary>
        //public void Place(GoMove[] moves)
        //{
        //    foreach (var m in moves)
        //    {
        //        WriteCommand("place_free_handicap", m.Color == GoColor.Black ? "black" : "white" + ' ' + m.Position);
        //        ReadResponse();
        //    }
        //}

        /// <summary>
        /// Used to make a move.  This returns immediately, since no real processing has to be done.
        /// </summary>
        public GoMoveResult Play(GoMove move)
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

                WriteCommand("play", (move.Color == GoColor.Black ? "black" : "white") + ' ' + position);
                ReadResponse();
            }
            // Add to move history and record new game state in database so user can 
            // see what happened.
            var rval = AddMoveAndUpdateStateAndSaveToDatabase(move);

            return rval;
        }

        public void Undo()
        {
            try
            {
                // Note that resignation is stored as a single move, but fuego.exe doesn't know about resignations so
                // no need to send an undo command to the engine.

                int undo = 0;

                if (State.Status == GoGameStatus.BlackWonDueToResignation)
                {
                    var humanColor = State.Player1.PlayerType == PlayerType.Human ? GoColor.Black : GoColor.White;
                    undo = humanColor == GoColor.Black ? 2 : 1;

                    if (State.GoMoveHistory.Count > 1 && State.GoMoveHistory[State.GoMoveHistory.Count - 2].Move.Color == humanColor)
                    {
                        WriteCommand("gg-undo", "1");
                        ReadResponse();
                    }
                }
                else if (State.Status == GoGameStatus.WhiteWonDueToResignation)
                {
                    var humanColor = State.Player1.PlayerType == PlayerType.Human ? GoColor.Black : GoColor.White;
                    undo = humanColor == GoColor.White ? 2 : 1;

                    if (State.GoMoveHistory.Count > 1 && State.GoMoveHistory[State.GoMoveHistory.Count - 2].Move.Color == humanColor)
                    {
                        WriteCommand("gg-undo", "1");
                        ReadResponse();
                    }
                }
                else
                {
                    var his = State.GoMoveHistory;
                    var count = his.Count;

                    var humanColor = State.Player1.PlayerType == PlayerType.Human ? GoColor.Black : GoColor.White;

                    // Reverse to before most recent human move.
                    for (int i = count - 1; i >= 0; i--)
                    {
                        if (his[i].Move.Color == humanColor)
                        {
                            undo = count - i;
                            break;
                        }
                    }

                    if (undo == 0)
                        throw new Exception("Can't undo because there are no human moves yet.");

                    WriteCommand("gg-undo", undo.ToString(CultureInfo.InvariantCulture));
                    ReadResponse();
                }

                UndoMovesInStateAndDatabase(undo);
            }
            catch (Exception)
            {
                // Setting State to null forces it to be loaded from database when the next user request
                // is made to the service.
                State = null;
                throw;
            }
        }

        public string SaveSGF()
        {
            try
            {
                var tmpFilename = Path.GetTempFileName();
                WriteCommand("savesgf", '"' + tmpFilename + '"');
                ReadResponse();
                var sgf = File.ReadAllText(tmpFilename, Encoding.UTF8);
                return sgf;
            }
            catch (Exception)
            {
                // Setting State to null forces it to be loaded from database when the next user request
                // is made to the service.
                State = null;
                throw;
            }
        }

        public void LoadSGF(string sgf)
        {
            try
            {
                var tmpFilename = Path.GetTempFileName();
                File.WriteAllText(tmpFilename, sgf);
                WriteCommand("loadsgf", '"' + tmpFilename + '"');
                ReadResponse();
                UpdateStateFromExeAndSaveToDatabase();
            }
            catch (Exception)
            {
                // Setting State to null forces it to be loaded from database when the next user request
                // is made to the service.
                State = null;
                throw;
            }
        }

        //public void Place(IEnumerable<string> positions, GoColor color)
        //{
        //    GoMove[] moves = positions.Select(m => new GoMove(MoveType.Normal, color, m)).ToArray();
        //    Place(moves);
        //}

        public void GenMove(GoColor color, out GoMove newMove, out GoMoveResult result)
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
            
            WriteCommand("genmove", color == GoColor.Black ? "black" : "white");
            string code, msg;
            ReadResponse(out code, out msg);

            switch (msg)
            {
                case "PASS":
                    newMove = new GoMove(MoveType.Pass, color, null);
                    break;
                case "resign":
                    newMove = new GoMove(MoveType.Resign, color, null);
                    break;
                default:
                    newMove = new GoMove(MoveType.Normal, color, msg);
                    break;
            }

            // Add to move history and record new game state in database so user can 
            // see what happened.
            //Thread.Sleep(30000);
            result = AddMoveAndUpdateStateAndSaveToDatabase(newMove);
        }

        public GoMove Hint(GoColor color)
        {
            WriteCommand("reg_genmove", color == GoColor.Black ? "black" : "white");
            string code, msg;
            ReadResponse(out code, out msg);

            GoMove hint;
            switch (msg)
            {
                case "PASS":
                    hint = new GoMove(MoveType.Pass, color, null);
                    break;
                case "resign":
                    hint = new GoMove(MoveType.Resign, color, null);
                    break;
                default:
                    hint = new GoMove(MoveType.Normal, color, msg);
                    break;
            }

            return hint;
        }

        //public GoMoveResult Undo()
        //{
        //    WriteCommand("undo");
        //    ReadResponse();
        //    SaveStones();
        //    var rval = new GoMoveResult();
        //    return rval;
        //}

//        public void Quit()
//        {
//            WriteCommand("quit");
//            ReadResponse();
//            if (Process != null)
//            {
//                try
//                {
//                    Process.Kill();
//                }
//// ReSharper disable EmptyGeneralCatchClause
//                catch
//// ReSharper restore EmptyGeneralCatchClause
//                {
//                }
//            }
//            Process = null;
//        }

        //public string Name()
        //{
        //    WriteCommand("name");
        //    string code, msg;
        //    ReadResponse(out code, out msg);
        //    return msg;
        //}

        //public string Version()
        //{
        //    WriteCommand("version");
        //    string code, msg;
        //    ReadResponse(out code, out msg);
        //    return msg;
        //}

        //public bool IsFunctioning
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (Process == null)
        //                return false;
        //            if (!Process.Responding)
        //                return false;
        //            if (Process.HasExited)
        //                return false;
        //            if (Process.ExitTime > Process.StartTime)
        //                return true;
        //            Name(); // if this causes an exception, we're in a bad state
        //        }
        //        catch (Exception)
        //        {
        //            return false;
        //        }

        //        return true;
        //    }
        //}

        ///// <summary>
        ///// Engine must be running or this will throw an error.
        ///// </summary>
        //public bool CompareEngineState(GoGameState clientState)
        //{
        //    // Get black and white positions from exe.
        //    SaveStones();

        //    // This does a deep comparison.
        //    var rval = Equals(clientState, _state);
        //    return rval;
        //}

        #endregion Public Methods

        #region Private Helpers

        private void ReopenGame()
        {
            try
            {
                State.WinMargin = 0;
                State.Status = GoGameStatus.Active;

                // Save to database.
                _goGRepository.SaveGameState(CurrentGameId, State);
            }
            catch (DbEntityValidationException)
            {
                // Setting State to null will cause it to be loaded from database the next time
                // a client initiates an operation.
                State = null;
                throw;
            }
        }

        private void UndoMovesInStateAndDatabase(int moves)
        {
            try
            {
                GetStones(); // Gets the new _state.BlackPositions and _state.WhitePositions.

                if (State.GoMoveHistory == null)
                    State.GoMoveHistory = new List<GoMoveHistoryItem>();
                for (int i = 0; i < moves; i++)
                    State.GoMoveHistory.RemoveAt(State.GoMoveHistory.Count - 1);

                // Change turn if an odd number of moves were undone.
                if (moves % 2 == 1)
                    State.WhoseTurn = State.WhoseTurn == GoColor.Black ? GoColor.White : GoColor.Black;

                State.WinMargin = 0;
                State.Status = GoGameStatus.Active;

                // Save to database.
                _goGRepository.SaveGameState(CurrentGameId, State);
            }
            catch (DbEntityValidationException)
            {
                throw;
            }
        }

        private GoMoveResult AddMoveAndUpdateStateAndSaveToDatabase(GoMove move)
        {
            try
            {
                var beforeBlack = State.BlackPositions;
                var beforeWhite = State.WhitePositions;

                GetStones(); // Gets the new _state.BlackPositions and _state.WhitePositions.

                GoMoveResult rval;
                if (move.Color == GoColor.Black)
                    rval = new GoMoveResult(beforeWhite, State.WhitePositions);
                else
                    rval = new GoMoveResult(beforeBlack, State.BlackPositions);

                if (State.GoMoveHistory == null)
                    State.GoMoveHistory = new List<GoMoveHistoryItem>();
                State.GoMoveHistory.Add(new GoMoveHistoryItem { Move = move, Result = rval });

                // Change turn.
                State.WhoseTurn = State.WhoseTurn == GoColor.Black ? GoColor.White : GoColor.Black;

                switch (move.MoveType)
                {
                    case MoveType.Resign:
                        State.Status = move.Color == GoColor.Black
                            ? GoGameStatus.WhiteWonDueToResignation
                            : GoGameStatus.BlackWonDueToResignation;
                        //var gameResult = CalculateGameResult();
                        //State.WinMargin = Decimal.Parse(gameResult.Substring(1));
                        break;
                    case MoveType.Pass:
                        // If previous move was a pass also, calculate winner.
                        var moveCount = State.GoMoveHistory.Count;
                        bool previousMoveWasPass = moveCount >= 2 &&
                                                   State.GoMoveHistory[moveCount - 2].Move.MoveType == MoveType.Pass;
                        if (previousMoveWasPass)
                        {
                            var gameResult2 = CalculateGameResult();
                            State.WinMargin = Decimal.Parse(gameResult2.Substring(1));
                            State.Status = gameResult2.StartsWith("B") ? GoGameStatus.BlackWon : GoGameStatus.WhiteWon;
                        }
                        break;
                }

                rval.Status = State.Status;
                rval.WinMargin = State.WinMargin;

                // Save to database.
                _goGRepository.SaveGameState(CurrentGameId, State);

                return rval;
            }
            catch (DbEntityValidationException)
            {
                // Setting State to null will cause it to be loaded from database the next time
                // a client initiates an operation.
                State = null;
                throw;
            }
        }

        private void UpdateStateFromExeAndSaveToDatabase()
        {
            try
            {
                WriteCommand("showboard");
                ReadResponse();
                var msg = new StringBuilder();
                foreach (var l in _debugLines)
                {

                }                   

                GetStones(); // Gets the new _state.BlackPositions and _state.WhitePositions.
                
                if (State.GoMoveHistory == null)
                    State.GoMoveHistory = new List<GoMoveHistoryItem>();
                
                // Determine whose turn.
                State.WhoseTurn = State.WhoseTurn == GoColor.Black ? GoColor.White : GoColor.Black;

                //switch (move.MoveType)
                //{
                //    case MoveType.Resign:
                //        State.Status = move.Color == GoColor.Black
                //            ? GoGameStatus.WhiteWonDueToResignation
                //            : GoGameStatus.BlackWonDueToResignation;
                //        //var gameResult = CalculateGameResult();
                //        //State.WinMargin = Decimal.Parse(gameResult.Substring(1));
                //        break;
                //    case MoveType.Pass:
                //        // If previous move was a pass also, calculate winner.
                //        var moveCount = State.GoMoveHistory.Count;
                //        bool previousMoveWasPass = moveCount >= 2 &&
                //                                   State.GoMoveHistory[moveCount - 2].Move.MoveType == MoveType.Pass;
                //        if (previousMoveWasPass)
                //        {
                //            var gameResult2 = CalculateGameResult();
                //            State.WinMargin = Decimal.Parse(gameResult2.Substring(1));
                //            State.Status = gameResult2.StartsWith("B") ? GoGameStatus.BlackWon : GoGameStatus.WhiteWon;
                //        }
                //        break;
                //}

                //rval.Status = State.Status;
                //rval.WinMargin = State.WinMargin;

                // Save to database.
                _goGRepository.SaveGameState(CurrentGameId, State);

                //return rval;
            }
            catch (DbEntityValidationException)
            {
                // Setting State to null will cause it to be loaded from database the next time
                // a client initiates an operation.
                State = null;
                throw;
            }
        }

        private string CalculateGameResult()
        {
            WriteCommand("final_score", null);
            string code, msg;
            ReadResponse(out code, out msg);

            return msg;
        }

        private void WriteCommand(string cmd, string value = null)
        {
#if DEBUG
            _logger.LogGameInfo(this.CurrentGameId, "WRITING: " + cmd + ' ' + (value ?? String.Empty) + '\n');
            Debug.Write("WRITING: ");
            Debug.Write(cmd);
#endif
            _writer.Write(cmd);
            if (value != null)
            {
                Debug.Write(' ');
                _writer.Write(' ');
                Debug.Write(value.ToString());
                _writer.Write(value.ToString());
            }
#if DEBUG
            Debug.Write("\n\n");
#endif
            _writer.Write("\n\n");
            _writer.Flush();
            Thread.Sleep(10);
        }

        /// <summary>
        /// Overload for when you don't need the code and msg.
        /// </summary>
        private void ReadResponse()
        {
            string code;
            string msg;
            ReadResponse(out code, out msg);
        }

        /// <summary>
        /// Puts the returned id and message into variables _id and _message.  Throws a 
        /// GoEnginException if Fuego complains.
        /// </summary>
        private void ReadResponse(out string code, out string msg)
        {
            code = null;
            msg = null;

            _debugLines.Clear();

            var haveResult = false;
            while (true)
            {
                Thread.Sleep(50); // allow more text to come out
                
                while (!_inputs.IsEmpty)
                {
                    string line;
                    _inputs.TryDequeue(out line);
#if DEBUG
                    _logger.LogGameInfo(this.CurrentGameId, "Read: " + (line ?? "(NULL)"));
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
                                ParseEngineOutput(line, out code, out msg);
                                ParseErrorAndThrowException(code, msg);
                                break;
                            case '=':
                                // If line starts with '=', no error.
                                haveResult = true;
                                ParseEngineOutput(line, out code, out msg);
                                break;
                            default:
                                // If line starts with something else, save it.
                                _debugLines.Add(line);
                                break;
                        }
                    }
                }
                
                // Hopefully the above Thread.Sleep() delayed enough to get the full response.  The result line (starts with
                // = or ?) can come first, and we put all other lines in _debugLines.
                Thread.Sleep(50);
                if (_inputs.IsEmpty && haveResult)
                    break;
            }
        }

        readonly List<string> _debugLines = new List<string>();

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

            WriteCommand("showboard");
            ReadResponse();
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
        private void ParseEngineOutput(string rval, out string id, out string msg)
        {
            if (rval[1] == ' ')
            {
                // code is not present
                id = null;
                msg = rval.Substring(2);
            }
            else
            {
                // code is present
                var strpos = rval.IndexOf(' ', 2);
                id = rval.Substring(1, strpos - 1);
                msg = rval.Substring(strpos + 1);
            }
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            _inputs.Enqueue(args.Data);
        }

        [Conditional("DEBUG")]
        private static void DebugMessage(string message)
        {
#if DEBUG
            Debug.WriteLine(message);
#endif
        }

        /// <summary>
        /// Call after each move to save state.
        /// </summary>
        private void GetStones()
        {
            WriteCommand("list_stones", "black");
            string code, msg;
            ReadResponse(out code, out msg);
            State.BlackPositions = msg;

            WriteCommand("list_stones", "white  ");
            ReadResponse(out code, out msg);
            State.WhitePositions = msg;
        }

        #endregion Private Helpers


        //        void IGoEngine.Setup(GoColor color, uint x, uint y)
        //        {
        //            if (!InitEngine())
        //                return;
        //            writer.WriteCommand("play", color, x, y, _size);
        //            Thread.Sleep(SLEEP_COMM);
        //            GetResponse();
        //        }

        //        GoMove IGoEngine.Reply(GoColor color)
        //        {
        //            if (!InitEngine())
        //                return new GoMovePass();
        //            writer.WriteCommand("genmove", color);
        //            Thread.Sleep(SLEEP_COMM);
        //            GetResponse();
        //            reader.ReadVertex(_size);
        //            if (reader.Pass)
        //                return new GoMovePass();
        //            if (reader.Resign)
        //                return new GoMoveResign();
        //            return new GoMove(reader.Color == GtpColor.Black ? GoColor.Black : GoColor.White, reader.X, reader.Y);
        //        }

        //        void IGoEngine.Play(GoMove move)
        //        {
        //            if (!InitEngine())
        //                return;
        //            if (move is GoMovePass || move is GoMoveResign)
        //            {
        //                writer.WriteCommandStart("play");
        //                writer.Write(move.Color == GoColor.Black ? GtpColor.Black : GtpColor.White);
        //                writer.Write(move is GoMovePass ? GtpPassResign.Pass : GtpPassResign.Resign);
        //                writer.WriteCommandEnd();
        //            }
        //            else
        //                writer.WriteCommand("play", move.Color == GoColor.Black ? GtpColor.Black : GtpColor.White, move.X, move.Y, _size);
        //            Thread.Sleep(SLEEP_COMM);
        //            GetResponse();
        //        }

        //        void IGoEngine.Undo()
        //        {
        //            if (!InitEngine())
        //                return;
        //            writer.WriteCommand("undo");
        //            Thread.Sleep(SLEEP_COMM);
        //            GetResponse();
        //        }

        //        void IGoEngine.Quit()
        //        {
        //            if (!_inited)
        //                return;
        //            _inited = false;
        //            try
        //            {
        //                writer.WriteCommand("quit");
        //                Process.Kill();
        //            }
        //            catch
        //            {
        //            }
        //            _inited = false;
        //#if DEBUG
        //            if (log != null)
        //                try
        //                {
        //                    log.Close();
        //                }
        //                catch (Exception)
        //                {
        //                }

        //            log = null;
        //#endif
        //        }

        //        GoScore IAdvancedGoEngine.Score() // estimate_score
        //        {
        //            if (!InitEngine())
        //                return new GoScore(GoColor.Black, 0.0F);
        //            writer.WriteCommand("estimate_score");
        //            Thread.Sleep(SLEEP_COMM);
        //            GetResponse();
        //            if (!reader.ReadString())
        //                throw new GtpException("Command: 'estimate_score' did not return a string.");
        //            string score = reader.String;
        //            reader.ReadEnd();
        //            GoColor winner = (score[0] == 'W' ? GoColor.White : GoColor.Black);
        //            score = score.Substring(1);
        //            return new GoScore(winner, float.Parse(score, CultureInfo.InvariantCulture));
        //        }

        //        GoPoint[] IAdvancedGoEngine.Dead() // final_status_list dead
        //        {
        //            if (!InitEngine())
        //                return new GoPoint[0];
        //            ArrayList list = new ArrayList();
        //            writer.WriteCommand("final_status_list", "dead");
        //            Thread.Sleep(SLEEP_COMM);
        //            GetResponse();
        //            do
        //            {
        //                while (reader.ReadVertex(_size))
        //                    list.Add(new GoPoint(reader.X, reader.Y));
        //            } while (reader.ReadNewline());
        //            return (GoPoint[])list.ToArray(typeof(GoPoint));
        //        }

        //        GoPoint[] IAdvancedGoEngine.Territory(GoColor color) // final_status_list black_territory
        //        {
        //            if (!InitEngine())
        //                return new GoPoint[0];
        //            ArrayList list = new ArrayList();
        //            writer.WriteCommand("final_status_list", (color == GoColor.Black ? "black" : "white") + "_territory");
        //            Thread.Sleep(SLEEP_COMM);
        //            GetResponse();
        //            do
        //            {
        //                while (reader.ReadVertex(_size))
        //                    list.Add(new GoPoint(reader.X, reader.Y));
        //            } while (reader.ReadNewline());
        //            return (GoPoint[])list.ToArray(typeof(GoPoint));
        //        }

        //GoPoint[] IAdvancedGoEngine.Ideas(GoColor color) // top_moves_black/top_moves_white
        //{
        //    if (!InitEngine())
        //        return new GoPoint[0];
        //    ArrayList list = new ArrayList();
        //    WriteCommand("top_moves_" + (color == GoColor.Black ? "black" : "white"));
        //    Thread.Sleep(SLEEP_COMM);
        //    GetResponse();
        //    do
        //    {
        //        while (reader.ReadVertex(_size))
        //        {
        //            list.Add(new GoPoint(reader.X, reader.Y));
        //            reader.ReadFloat(); // skip value
        //        }
        //    } while (reader.ReadNewline());
        //    return (GoPoint[])list.ToArray(typeof(GoPoint));
        //}

        //public void AbortRead()
        //{
        //    reader.AbortRead();
        //}

        //private void GetResponse()
        //{
        //    reader.ReadResponse();
        //    if (reader.IsError)
        //        throw new GtpException("Error response: " + reader.ErrorMessage);
        //}

        //public bool Ping()
        //{
        //    ping = false;
        //    ThreadPool.QueueUserWorkItem(new WaitCallback(PingEngine));
        //    for (int i = 0; i < 20; i++)
        //    {
        //        if (ping)
        //            return true;
        //        Thread.Sleep(100);
        //    }
        //    return false;
        //}
    }
}
