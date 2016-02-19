using System;

namespace GoG.ServerCommon.Logging
{
    public interface ILogger
    {
        void LogServerError(Guid gameId, Exception ex, object context);
        void LogEngineException(Guid gameId, Exception ex, object context);
        void LogGameInfo(Guid gameId, string txt);
    }

    public enum LogLevel
    {
        Info, Warning, Error
    }
}