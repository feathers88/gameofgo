using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using GoG.Infrastructure.Engine;
using GoG.Repository.Engine;
using GoG.Repository.Log;
using GoG.ServerCommon.Logging;
using GoEngineException = GoG.Infrastructure.Services.Engine.GoEngineException;

namespace GoG.Fuego
{
    /// <summary>
    /// Runs multiple instances of fuego.exe, and routes requests to them.
    /// </summary>
    public sealed class FuegoEngine
    {
        public static FuegoEngine Instance = null;

        #region Data
        private ILogger _logger;

        private static readonly object Lock = new object();
        private HashSet<FuegoInstance> _instances;
        private IGoGRepository _goGRepository;
        private TimeSpan _cleanupInterval;
        private byte _maxInstances;
        private long _memoryPerInstance;
        private DateTimeOffset? _lastCleanup;

        internal static readonly string ExePath;
        #endregion Data

        #region Ctor and init / cleanup

        static FuegoEngine()
        {
            new DbLogRepository().Log(Guid.Empty, LogLevel.Info, "Entering FuegoEngine static constructor.", null);

            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase.Substring(8));
            ExePath = dir + @"\Fuego.exe";
            if (!File.Exists(ExePath))
                throw new GoEngineException(GoResultCode.ServerInternalError, "Fuego.exe was not found on path \"" + ExePath + "\".");
        }

        private FuegoEngine()
        {
        }
        
        public static void Init(ILogger logger, IGoGRepository goGRepository)
        {
            // Lock is static.  This prevents multiple requests from trying to both initialize.  Instead,
            // the first request will do the initialization, and the subsequents requests will get stuck
            // here until the first request is finished initializing.  We can't just return immediately on the
            // subsequent requests because they would try to use the not-yet-fully-initialized "Instance" and 
            // cause an exception or invalid state.
            lock (Lock)
            {
                if (Instance != null)
                    return;

                Instance = new FuegoEngine();
                Instance.Setup(logger, goGRepository);
            }
        }

        private void Setup(ILogger logger, IGoGRepository goGRepository)
        {

            _logger = logger;
            _goGRepository = goGRepository;

            var cleanupInMinutess = Convert.ToInt32(ConfigurationManager.AppSettings["CleanupIntervalInMinutes"]);
            _cleanupInterval = TimeSpan.FromMinutes(cleanupInMinutess);
            _instances = new HashSet<FuegoInstance>();
            _maxInstances = Convert.ToByte(ConfigurationManager.AppSettings["FuegoInstances"]);
            var gb = Convert.ToDecimal(ConfigurationManager.AppSettings["GBPerFuegoInstance"]);
            _memoryPerInstance = (long) Decimal.Round((decimal) 1024*1024*1024*gb);

            Cleanup();

            // Spin up copies of fuego.exe up to instance limit.
            for (int i = 0; i < Instance._maxInstances; i++)
            {
                var instance = new FuegoInstance(_goGRepository, _memoryPerInstance, _logger);
                _instances.Add(instance);
            }
        }

        public void Cleanup()
        {
            if (_lastCleanup == null || DateTimeOffset.Now > _lastCleanup + _cleanupInterval)
            {
                // Stop all orphaned fuego.exe instances (not in our _tokens list).
                var runningFuegos = Process.GetProcessesByName("fuego.exe");
                foreach (var runningFuego in runningFuegos)
                {
                    var match = _instances.FirstOrDefault(token => token.Process.Id == runningFuego.Id);
                    if (match == null)
                        runningFuego.Kill();
                }

                _lastCleanup = DateTimeOffset.Now;
            }
        }

        #endregion Ctor and init / cleanup

        public void Start(Guid gameId, GoGameState state)
        {
            if (state == null) throw new ArgumentNullException("state");

            bool exists = _goGRepository.GetGameExists(gameId);

            if (exists)
                throw new GoEngineException(GoResultCode.GameAlreadyExists, null);

            // Get a Fuego instance and start it up.
            FuegoInstance inst = null;
            try
            {
                inst = GetFuegoInstance(gameId, GoOperation.Starting);
                inst.Start(state);

                // Save as the new game state.
                _goGRepository.SaveGameState(gameId, state);
            }
// ReSharper disable RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
// ReSharper restore RedundantCatchClause
            finally
            {
                if (inst != null) 
                    inst.CurrentOperation = GoOperation.Idle;
            }
        }

        public GoMoveResult Play(Guid gameId, GoMove move)
        {
            if (move == null) throw new ArgumentNullException("move");

            // Get a Fuego instance and start it up.
            FuegoInstance inst = null;
            try
            {
                GoOperation op;
                switch (move.MoveType)
                {
                    case MoveType.Normal:
                        op = GoOperation.NormalMove;
                        break;
                    case MoveType.Pass:
                        op = GoOperation.Pass;
                        break;
                   case MoveType.Resign:
                        op = GoOperation.Resign;
                        break;
                    default:
                        throw new Exception("Unrecognized move type: " + move.MoveType);
                }
                inst = GetFuegoInstance(gameId, op);
                inst.EnsureRunning();
                var result = inst.Play(move);

                return result;
            }
// ReSharper disable RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
// ReSharper restore RedundantCatchClause
            finally
            {
                if (inst != null)
                    inst.CurrentOperation = GoOperation.Idle;
            }
        }

        public void GenMove(Guid gameId, GoColor color, out GoMove newMove, out GoMoveResult result)
        {
            // Get a Fuego instance and start it up.
            FuegoInstance inst = null;
            try
            {
                inst = GetFuegoInstance(gameId, GoOperation.GenMove);
                inst.EnsureRunning();
                inst.GenMove(color, out newMove, out result);
            }
// ReSharper disable RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
// ReSharper restore RedundantCatchClause
            finally
            {
                if (inst != null)
                    inst.CurrentOperation = GoOperation.Idle;
            }
        }

        public GoMove Hint(Guid gameId, GoColor color)
        {
            // Get a Fuego instance and start it up.
            FuegoInstance inst = null;
            try
            {
                inst = GetFuegoInstance(gameId, GoOperation.Hint);
                inst.EnsureRunning();
                var result = inst.Hint(color);

                return result;
            }
            // ReSharper disable RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
            // ReSharper restore RedundantCatchClause
            finally
            {
                if (inst != null)
                    inst.CurrentOperation = GoOperation.Idle;
            }
        }

        public GoGameState Undo(Guid gameId)
        {
            // Get a Fuego instance and start it up.
            FuegoInstance inst = null;
            try
            {
                inst = GetFuegoInstance(gameId, GoOperation.Undo);
                inst.EnsureRunning();
                inst.Undo();
                inst.CurrentOperation = GoOperation.Idle;

                return _goGRepository.GetGameState(gameId);
            }
            // ReSharper disable RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
            // ReSharper restore RedundantCatchClause
            finally
            {
                if (inst != null)
                    inst.CurrentOperation = GoOperation.Idle;
            }
        }

        public string SaveSGF(Guid gameId)
        {
            // Get a Fuego instance and start it up.
            FuegoInstance inst = null;
            try
            {
                inst = GetFuegoInstance(gameId, GoOperation.Undo);
                inst.EnsureRunning();
                var svg = inst.SaveSGF();

                return svg;
            }
            // ReSharper disable RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
            // ReSharper restore RedundantCatchClause
            finally
            {
                if (inst != null)
                    inst.CurrentOperation = GoOperation.Idle;
            }
        }

        public void LoadSGF(Guid gameId, string sgf)
        {
            // Get a Fuego instance and start it up.
            FuegoInstance inst = null;
            try
            {
                inst = GetFuegoInstance(gameId, GoOperation.Undo);
                inst.EnsureRunning();
                inst.LoadSGF(sgf);
            }
            // ReSharper disable RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
            // ReSharper restore RedundantCatchClause
            finally
            {
                if (inst != null)
                    inst.CurrentOperation = GoOperation.Idle;
            }
        }

        private FuegoInstance GetFuegoInstance(Guid gameId, GoOperation operation)
        {
            //lock (_lock)
            //{
                // 1.  Look for a running instance matching our gameid.
                var matchingInstance = _instances.FirstOrDefault(i => i.CurrentGameId == gameId);
                if (matchingInstance != null)
                {
                    if (matchingInstance.CurrentOperation != GoOperation.Idle)
                        throw new GoEngineException(GoResultCode.SimultaneousRequests, "Simultaneous requests for gameId " + gameId + '.');
                    matchingInstance.CurrentOperation = operation;
                    return matchingInstance;
                }

                // 2.  Look for an instance that isn't processing.
                var ordered = _instances.OrderBy(i => i.LastOperationFinished).ToArray();
                var nonProcessingInstance = ordered.FirstOrDefault(i => i.CurrentOperation == GoOperation.Idle);
                if (nonProcessingInstance != null)
                {
                    nonProcessingInstance.CurrentGameId = gameId;
                    nonProcessingInstance.State = null; // Forces a load from database later.
                    nonProcessingInstance.CurrentOperation = operation;
                    return nonProcessingInstance;
                }

                // 3.  We're boned.  All instances are processing other requests.
                throw new GoEngineException(GoResultCode.EngineBusy,
                                            "Engine exceeded maximum number of simultaneous requests (" + _instances.Count + '/' + _maxInstances + ")!");
            //}
        }
        
        //public GoGameState GetState(Guid gameId)
        //{
        //    var state = _goGRepository.GetGameState(gameId);
        //    return state;
        //}


        public GoOperation GetCurrentOperation(Guid gameId)
        {
            // Get a Fuego instance and start it up.
            try
            {
                var inst = _instances.FirstOrDefault(i => i.CurrentGameId == gameId);
                return inst == null ? GoOperation.Idle : inst.CurrentOperation;
            }
// ReSharper disable RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
// ReSharper restore RedundantCatchClause
        }
    }
}
