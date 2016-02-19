using System;
using System.Runtime.Serialization;
using GoG.Infrastructure.Engine;

namespace GoG.Infrastructure.Services.Engine
{
    /// <summary>
    /// This is the basis for all responses to the client by IMultoGoEngine.
    /// </summary>
    [DataContract]
    public class GoResponse
    {
        // This empty constructor is so WCF's DataContractSerializer is able to build an instance of this type.
        public GoResponse()
        {
            ResultCode = GoResultCode.Success;
        }

        public GoResponse(GoResultCode resultCode)
        {
            ResultCode = resultCode;
        }

        /// <summary>
        /// A code to be sent to the client.
        /// </summary>
        [DataMember]
        public GoResultCode ResultCode { get; set; }
    }
}