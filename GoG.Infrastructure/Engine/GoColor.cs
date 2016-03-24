using System.Runtime.Serialization;

namespace GoG.Infrastructure.Engine
{
    /// <summary>
    /// Stone color.  Black always goes first, but White gets a scoring edge called "Komi".
    /// </summary>
    public enum GoColor
    {
        [EnumMember]
        Black,
        [EnumMember]
        White
    }
}