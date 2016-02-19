using GoG.Infrastructure.Engine;
using System;

namespace GoG.Infrastructure.Services.Engine
{
    /// <summary>
    /// These exceptions are used inside the engine, _not_ transferred to the client.
    /// The WCF service catches all exceptions and puts Code into GoResponse.ResultCode.  The 
    /// Message part of the exception is used for logging only (the service does the logging).
    /// </summary>
    public class GoEngineException : Exception
    {
        public GoResultCode Code { get; set; }

        public GoEngineException(GoResultCode code, string msg)
            : base(msg)
        {
            Code = code;
        }
    }
}
