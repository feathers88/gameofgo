using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoG.WinRT.ViewModels.IgsProtocol
{
    public enum States
    {
        ConnectionClosed,
        Connected,
        SentUserId,
        SentPassword,
        LoggedIn,
        Disconnecting,
        SentInitialSetupCommands,
        SentInitialRoomList2Command,
        GotROOMLIST2Response,
    }
}
