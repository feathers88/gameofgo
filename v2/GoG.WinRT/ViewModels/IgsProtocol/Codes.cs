namespace GoG.WinRT.ViewModels.IgsProtocol
{
    /* 
           These are the possible values for in parray.session.protostate.
           They are used as the first second number in the client-type numerical prompt.
           For more information on these #$%^$%## defines, look at client source
           code, such as xigc.  What a horrible way to live.
           Renamed these to CODE_ , to make them more greppable. AvK
        */
    public enum Codes
    {
        // These are the codes I believe are correct.
        // If any code is wrong, I comment it below and put it here with the correct value. I
        // also add new enumerations if I don't see an obvious analog below.
        CODE_NONE = 0,
        CODE_PROMPT = 1,
        CODE_LOGGEDIN = 39,


        // The rest of these are from the NNGS source code, and is out of date and often wrong.
        CODE_BEEP = 2,
        CODE_DOWN = 4,
        CODE_ERROR = 5,
        //CODE_FILE = 6,  // apparently CODE_INFO is used instead of a dedicated code ("9 File").
        CODE_GAMES = 7,
        CODE_HELP = 8,
        CODE_INFO = 9,
        CODE_LAST = 10,
        CODE_KIBITZ = 11,
        CODE_LOAD = 12,
        CODE_LOOK_M = 13,
        CODE_MESSAGE = 14,
        CODE_MOVE = 15,
        CODE_OBSERVE = 16,
        CODE_REFRESH = 17,
        CODE_SAVED = 18,
        CODE_SAY = 19,
        CODE_SCORE_M = 20,
        CODE_SHOUT = 21,
        CODE_STATUS = 22,
        CODE_STORED = 23,
        CODE_TELL = 24,
        CODE_THIST = 25,
        CODE_TIME = 26,
        CODE_WHO = 27,
        CODE_UNDO = 28,
        CODE_SHOW = 29,
        CODE_TRANS = 30,
        CODE_YELL = 32,
        CODE_TEACH = 33,
        //CODE_MVERSION = 39,
        CODE_DOT = 40,
        CODE_CLIVRFY = 41,
        /* PEM: Removing stones. */
        CODE_REMOVED = 49,
        CODE_EMOTE = 500,
        CODE_EMOTETO = 501,
        CODE_PING = 502,
        /* AvK: Sendcodes can now be or'ed with CODE_CR1 to prepend a NL */
        CODE_MASK = 511,
        CODE_CR1 = 1024,

    }

    /* These are used for the second number on the protocol lines
    ** when the first number is '1' (CODE_PROMPT)
    ** AvK: 
    ** grabbed these (5-10) from playerdb.h
    ** Added 0-4 from IGS docs
    */
    public enum SubCodes
    {
        STAT_LOGON = 0, /* Unused */
        STAT_PASSWORD = 1,
        STAT_PASSWORD_NEW = 2, /* Unused */
        STAT_PASSWORD_CONFIRM = 3, /* Unused */
        STAT_REGISTER = 4, /* Unused */
        STAT_WAITING = 5,
        STAT_PLAYING_GO = 6,
        STAT_SCORING = 7,
        STAT_OBSERVING = 8,
        STAT_TEACHING = 9, /* Unused */
        STAT_COMPLETE = 10, /* Unused */
    }
}
