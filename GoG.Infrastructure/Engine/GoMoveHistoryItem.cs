using System.Runtime.Serialization;

namespace GoG.Infrastructure.Engine
{
    [DataContract]
    public class GoMoveHistoryItem
    {
        [DataMember]
        public GoMove Move { get; set; }
        [DataMember]
        public int Sequence { get; set; }
        [DataMember]
        public GoMoveResult Result { get; set; }
    }
}
