using System;
using GoG.Repository.Log;
using GoG.ServerCommon.Logging;

namespace GoG.Services.Logging
{
    public class Logger : ILogger
    {
        #region Data
        private readonly ILogRepository _logRepo;
        #endregion Data

        #region Ctor

        public Logger()
        {
            _logRepo = new DbLogRepository();
        }

        public Logger(ILogRepository logRepo)
        {
            _logRepo = logRepo;
        }

        #endregion Ctor

        #region Public Methods

        public void Log(Guid gameId, LogLevel level, Exception ex, object context)
        {
            if (ex == null && context == null) throw new ArgumentException("'ex' and 'context' arguments cannot both be null.");

            if (context == null)
                context = ex.Message;

            _logRepo.Log(gameId, level, context, ex);
        }

        public void LogServerError(Guid gameId, Exception ex, object context)
        {
            try
            {
                Log(gameId, LogLevel.Error, ex, context);
            }
            catch
            {
            }
        }

        public void LogEngineException(Guid gameId, Exception ex, object context)
        {
            try
            {
                Log(gameId, LogLevel.Warning, ex, context);
            }
            catch
            {
            }
        }

        public void LogGameInfo(Guid gameId, string txt)
        {
            try
            {
                Log(gameId, LogLevel.Info, null, txt);
            }
            catch
            {
            }
        }
        
        #endregion Public Methods
    }
}