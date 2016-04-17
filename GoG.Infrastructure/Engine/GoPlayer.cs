using System.Runtime.Serialization;

namespace GoG.Infrastructure.Engine
{
    [DataContract]
    public class GoPlayer
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public PlayerType PlayerType { get; set; }
        [DataMember]
        public decimal Score { get; set; }
        [DataMember]
        public int Level { get; set; }
    }
}
