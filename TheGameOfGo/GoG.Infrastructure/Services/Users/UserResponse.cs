using System;

namespace GoG.Infrastructure.Services.Users
{
    /// <summary>
    /// All controller methods in the User service should serialize this back to client in the body.
    /// </summary>
    public class UserResponse
    {
        /// <summary>
        /// Must pass error codes in constructor.  It's impossible to add them later.
        /// </summary>
        /// <param name="initialCodes"></param>
        public UserResponse(params UserErrorCode[] initialCodes)
        {
            if (initialCodes != null && initialCodes.Length != 0)
            {
                _errorCodes = initialCodes;
                ErrorCodesAsString = GetErrorCodesAsOneString();
            }
        }

        private readonly UserErrorCode[] _errorCodes;
        public UserErrorCode[] ErrorCodes
        {
            get { return _errorCodes; }
        }

        public string ErrorCodesAsString { get; set; }

        private string GetErrorCodesAsOneString()
        {
            var rval = String.Empty;
            foreach (var code in ErrorCodes)
                rval += code + ' ';

            return rval.TrimEnd();
        }
    }
}
