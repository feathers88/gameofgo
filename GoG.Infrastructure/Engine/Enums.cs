using System.Runtime.Serialization;

namespace GoG.Infrastructure.Engine
{
    [DataContract]
    public enum GoOperation
    {
        [EnumMember]
        Idle,
        [EnumMember]
        Starting,
        [EnumMember]
        NormalMove,
        [EnumMember]
        Pass,
        [EnumMember]
        Resign,
        [EnumMember]
        GenMove,
        [EnumMember]
        Hint,
        [EnumMember]
        Undo
    }

    /// <summary>
    /// These enumerations correspond to the values in the Moves table MoveType field in the database.
    /// DO NOT change these Id values.
    /// </summary>
    [DataContract]
    public enum MoveType
    {
        [EnumMember]
        Normal = 0,
        [EnumMember]
        Pass = 1,
        [EnumMember]
        Resign = 2
    }

    [DataContract]
    public enum GoGameStatus
    {
        [EnumMember]
        Active = 0,
        [EnumMember]
        BlackWon = 1,
        [EnumMember]
        WhiteWon = 2,
        [EnumMember]
        BlackWonDueToResignation = 3,
        [EnumMember]
        WhiteWonDueToResignation = 4
    }

    [DataContract]
    public enum PlayerType
    {
        [EnumMember]
        Human = 0,
        [EnumMember]
        AI = 1,
        [EnumMember]
        Remote = 2
    }

    /// <summary>
    /// A code to be sent to the client.
    /// </summary>
    [DataContract]
    public enum GoResultCode
    {
        /// <summary>
        /// No error.
        /// </summary>
        [EnumMember]
        Success,

        /// <summary>
        /// Client side also generates these when there are connectivity errors, but 
        /// could be generated server side, too.
        /// </summary>
        [EnumMember]
        CommunicationError,

        /// <summary>
        /// An error happened on the server that the client shouldn't know about.
        /// </summary>
        [EnumMember]
        ServerInternalError,

        /// <summary>
        /// The GameId passed in doesn't exist (or doesn't belong to the current user).
        /// </summary>
        [EnumMember]
        GameDoesNotExist,

        /// <summary>
        /// The GameId passed in belongs to a game that already exists.
        /// </summary>
        [EnumMember]
        GameAlreadyExists,

        /// <summary>
        /// Game state between server and client does not match.  This usually means the client needs to correct the situation
        /// (or the client is trying to cheat).
        /// </summary>
        [EnumMember]
        ClientOutOfSync,

        /// <summary>
        /// Only one move or action can be performed for a single game at one time.  If multiple calls are made per game,
        /// they are rejected.
        /// </summary>
        [EnumMember]
        SimultaneousRequests,
        [EnumMember]
        IllegalMoveSpaceOccupied,
        [EnumMember]
        IllegalMoveSuicide,
        [EnumMember]
        IllegalMoveSuperKo,
        [EnumMember]
        OtherIllegalMove,

        /// <summary>
        /// Other error output by the engine.
        /// </summary>
        [EnumMember]
        OtherEngineError,

        /// <summary>
        /// This engine has reached the maximum number of players and can't handle any more right now.
        /// Currently this means the server can only start a certain number of instances of fuego.exe to
        /// move requests.  Clients should simply show a busy signal for 5 seconds, then
        /// try again.
        /// </summary>
        [EnumMember]
        EngineBusy,

        /// <summary>
        /// This an odd error output by the engine sometimes.  I suspect it may be when the engine is restricted
        /// to a very low number of "maximum simulated games" (used to limit its strength), but that's a guess.
        /// It's not clear how client client should handle this, so let's hope it doesn't happen too often.
        /// </summary>
        [EnumMember]
        CannotScore,
        [EnumMember]
        CannotSaveSGF
    }
}