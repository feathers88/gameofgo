using System.Collections.Generic;
using System.Runtime.Serialization;
using GoG.Infrastructure.Engine;

namespace GoG.Infrastructure.Services.Engine
{
    /// <summary>
    /// A list of positions response.
    /// </summary>
    [DataContract]
    public class GoHintResponse : GoResponse
    {
        // This empty constructor is so WCF's DataContractSerializer is able to build an instance of this type.
        public GoHintResponse()
        {
        }

        public GoHintResponse(GoResultCode resultCode, GoMove move)
            : base(resultCode)
        {
            Move = move;
        }
        
        [DataMember]
        public GoMove Move { get; set; }
    }
}