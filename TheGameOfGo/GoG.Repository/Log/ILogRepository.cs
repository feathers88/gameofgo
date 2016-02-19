using GoG.ServerCommon.Logging;
using System;

namespace GoG.Repository.Log
{
    public interface ILogRepository
    {
        void Log(Guid gameId, LogLevel level, object ctx, Exception ex);
    }

    
}
