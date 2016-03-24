using System.Runtime.Serialization;
using GoG.Infrastructure.Engine;

namespace GoG.Infrastructure.Services.Engine
{
    /// <summary>
    /// Sent to client in response to a score request.
    /// </summary>
    [DataContract]
    public class GoScoreResponse : GoResponse
    {
        // This empty constructor is so WCF's DataContractSerializer is able to build an instance of this type.
        public GoScoreResponse()
        {
        }

        public GoScoreResponse(GoResultCode resultCode, GoColor winner, float score)
            : base(resultCode)
        {
            Winner = winner;
            Score = score;
        }

        [DataMember]
        public GoColor Winner { get; set; }
        [DataMember]
        public float Score { get; set; }
    }
}