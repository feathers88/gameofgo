using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace GoG.Infrastructure.Engine
{
    /// <summary>
    /// Final score of a Go game, in relative terms. 
    /// TODO: Probably need to change this to just store White and Black scores.
    /// </summary>
    public class GoScore
    {
        public GoScore(GoColor winner, decimal score)
        {
            Debug.Assert(score >= 0);
            Winner = winner;
            Score = score;
        }

        public decimal ToFloat(GoColor color)
        {
            if (Winner != color)
                return -Score;
            return Score;
        }

        public GoColor Winner { get; set; }
        public decimal Score { get; set; }
    }
}

