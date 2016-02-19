using System;
using System.Net;

namespace GoG.Client.Exceptions
{
    public class HttpErrorCodeException : Exception
    {
        public HttpErrorCodeException(HttpStatusCode code, string error)
            : base(error)
        {
            HttpStatusCode = code;
        }

        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
