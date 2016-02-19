using System;
using GoG.Infrastructure.Engine;

namespace GoG.Infrastructure.Services.Users
{
    /// <summary>
    /// These exceptions are used inside the engine, _not_ transferred to the client.
    /// The WCF service catches all exceptions and puts Code into GoResponse.ResultCode.  The 
    /// Message part of the exception is used for logging only (the service does the logging).
    /// </summary>
    public class UsersException : Exception
    {
        public UserErrorCode Code { get; set; }

        public UsersException(UserErrorCode code)
        {
            Code = code;
        }

        public UsersException(UserErrorCode code, string msg)
            : base(msg)
        {
            Code = code;
        }
    }
}
