using System.Runtime.Serialization;

namespace GoG.Infrastructure.Engine
{
    public class GoMoveHistoryItem
    {
        public GoMove Move { get; set; }

        public int Sequence { get; set; }

        public GoMoveResult Result { get; set; }
    }
}
