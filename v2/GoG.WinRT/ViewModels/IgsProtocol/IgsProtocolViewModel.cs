using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using GoG.WinRT.Common;
using Microsoft.Practices.Prism.StoreApps;

namespace GoG.WinRT.ViewModels.IgsProtocol
{
    public class IgsProtocolViewModel : ViewModel, IDisposable
    {
        #region Ctor
        static IgsProtocolViewModel()
        {
            // We build this here so that we can use it later in ExtractCodeAndSubCode().
            // Our keys are unique, so we use the quicker sorted dictionary.
            LinePrefixes = new SortedDictionary<string, Command>();
            foreach (var val in Enum.GetValues(typeof(Codes)))
            {
                var code = (Codes)val;
                var i = (int)code;
                if (i == 1)
                {
                    // Add subcode for the 1 1, 1 5, etc.
                    foreach (var val2 in Enum.GetValues(typeof(SubCodes)))
                    {
                        var subCode = (SubCodes)val2;
                        var i2 = (int)subCode;
                        LinePrefixes.Add(string.Format("{0} {1} ", i, i2),
                                          new Command(code, subCode, null));
                    }
                }
                else
                    LinePrefixes.Add(string.Format("{0} ", i),
                                      new Command(code, null, null));
            }
        }

        public IgsProtocolViewModel(string host, ushort port)
        {
            _host = new HostName(host);
            _port = port;
        }
        #endregion Ctor

        #region Data
        // Used to store all possible IGS code combinations and their line prefixes so that
        // a quick lookup can be performed.
        private static readonly SortedDictionary<string, Command> LinePrefixes;
        // Saved for automatice reconnect.
        private readonly HostName _host;
        private readonly ushort _port;
        internal string UserId;
        internal string Password;
        // Used to communicate with the IGS server.
        private StreamSocket _socket;
        // Used by Disconnect() to cancel processing socket messages.
        private CancellationTokenSource _cts;
        // Used to store the excess msg data (a partial line without a final \r\n).
        private string _savedPartial = null;
        // Used to send and process multiple simple toggle commands.
        private ObservableCollection<string> _commandQueue = new ObservableCollection<string>();
        private List<Room> _tmpRooms;
        #endregion Data

        #region Properties

        private bool _isConnecting;
        public bool IsConnecting
        {
            get { return _isConnecting; }
            private set { _isConnecting = value; SetProperty(ref _isConnecting, value); }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            private set { _isConnected = value; SetProperty(ref _isConnected, value); }
        }

        private States _status;
        public States Status
        {
            get { return _status; }
            set { _status = value; SetProperty(ref _status, value); }
        }

        #endregion Properties

        #region Private Methods

        private async Task UpdateStateMachine(string msg)
        {
            // This method parses the reponse and maintains state in Status.  This method
            // fires event as necessary that the user of this class can listen to.

            var commands = SplitMessageIntoCommands(msg);
            foreach (var cmd in commands)
            {
                await ProcessCommand(cmd);

                // ProcessCommand have called Disconnect(), so we must check the cancellation token.
                if (!IsConnected || _cts.IsCancellationRequested)
                    return;
            }
        }

        private async Task ProcessCommand(Command cmd)
        {
            // Process the unimportant commands first, such as shouts and tells.
            switch (cmd.Code)
            {
                case Codes.CODE_SHOUT:
                case Codes.CODE_TELL:
                case Codes.CODE_NONE:
                    return;
            }

            switch (Status)
            {
                case States.Connected:
                    // "Login: " Prompt
                    if (cmd.Code == Codes.CODE_PROMPT && cmd.SubCode == SubCodes.STAT_WAITING)
                        return;
                    if (cmd.Code == Codes.CODE_PROMPT && cmd.SubCode == SubCodes.STAT_LOGON)
                    {
                        SendMessageAsync(UserId + "\r\n");
                        Status = States.SentUserId;
                    }
                    else
                    {
                        // If doesn't give us the "Login: " prompt, something went wrong and we
                        // abort the connection, firing events so the user of this class can 
                        // decide what to do.
                        InvokeUnexpectedResponseEvent(cmd);
                        Disconnect();
                        return;
                    }
                    break;
                case States.SentUserId:
                    if (cmd.Code == Codes.CODE_PROMPT && cmd.SubCode == SubCodes.STAT_WAITING)
                        return;
                    // 1 means PROMPT (for user input), the second 1 means PASSWORD.
                    if (cmd.Code == Codes.CODE_PROMPT && cmd.SubCode == SubCodes.STAT_PASSWORD)
                    {
                        SendMessageAsync(Password + "\r\n");
                        Status = States.SentPassword;
                    }
                    else
                    {
                        InvokeUnexpectedResponseEvent(cmd);
                        Disconnect();
                        return;
                    }
                    break;
                case States.SentPassword:
                    if (cmd.Code == Codes.CODE_PROMPT && cmd.SubCode == SubCodes.STAT_WAITING)
                        return;
                    if (cmd.Code == Codes.CODE_INFO && cmd.Text == "File")
                    {
                        // We get a couple of "9 File" responses while the introductory text prints out.  We
                        // must ignore these.
                        return;
                    }

                    if (cmd.Code == Codes.CODE_LOGGEDIN)
                    {
                        InvokeLoggedInEvent();
                        await SendMultipleCommands("toggle client true\r\n",
                                           "toggle review true\r\n",
                                           "toggle newrating true\r\n",
                                           "toggle nmatch true\r\n",
                                           "toggle seek true\r\n",
                                           "toggle newundo true\r\n");
                        Status = States.SentInitialSetupCommands;
                    }
                    else if (cmd.Code == Codes.CODE_ERROR)
                    {
                        if (cmd.Text == "Invalid password.")
                            InvokeInvalidPasswordEvent();
                        else
                            InvokeErrorEvent(cmd);
                        Disconnect();
                        return;
                    }
                    else
                    {
                        InvokeUnexpectedResponseEvent(cmd);
                        Disconnect();
                        return;
                    }
                    break;
                case States.SentInitialSetupCommands:
                    if (cmd.Code == Codes.CODE_PROMPT && cmd.SubCode == SubCodes.STAT_WAITING)
                        return;
                    if (ProcessCommandQueue(cmd))
                    {
                        SendMessageAsync("room list2\r\n");
                        Status = States.SentInitialRoomList2Command;
                    }
                    break;
                case States.SentInitialRoomList2Command:
                    if (cmd.Code == Codes.CODE_PROMPT && cmd.SubCode == SubCodes.STAT_WAITING)
                        return;
                    if (cmd.Code == Codes.CODE_INFO && cmd.Text == "ROOMLIST2")
                    {
                        // Create empty rooms list so we can start to fill it in the next status.
                        _tmpRooms = new List<Room>(20);
                        Status = States.GotROOMLIST2Response;
                    }
                    else if (cmd.Code == Codes.CODE_ERROR)
                    {
                        InvokeErrorEvent(cmd);
                        Disconnect();
                        return;
                    }
                    else
                    {
                        InvokeUnexpectedResponseEvent(cmd);
                        Disconnect();
                        return;
                    }
                    break;
                case States.GotROOMLIST2Response:
                    if (cmd.Code == Codes.CODE_PROMPT && cmd.SubCode == SubCodes.STAT_WAITING)
                    {
                        // input is complete
                        InvokeGotRoomsEvent(_tmpRooms);
                        // TODO: send next command and set next status...
                    }
                    if (cmd.Code == Codes.CODE_INFO && cmd.Text.StartsWith("ROOM "))
                    {
                        //9 ROOM 36(R): EuropeanTeamChamp;F;EuropeanTeamChamp;19;60;10;1,19,3600,300,300,15,0,0,0
                        //9 ROOM 37(R): AGA City League;F;AGA City League;19;60;10;1,19,3600,600,60,25,0,0,0
                        //9 ROOM 01(F): No rated games;PSMNYACF;...R......;19;1;10;1,19,60,600,30,25,10,60,0
                        //9 ROOM 93(F): PC beginners;PSMNYACT;..........K....;13;1;10;1,13,60,600,30,25,10,60,0
                        //9 ROOM 02(R): PairGO games;PSMNYAC;.y.A........;19;50;0
                        //9 ROOM 55(R): PRO LESSON;PSMNYAC;.v.....m.w....;19;50;0
                        //9 ROOM 58(R): TEACHING ROOM;F;.w......;19;40;0;1,19,2400,60,0,25,0,0,0;
                        //9 ROOM 83(F): Beginners(Robots);F;...{.b.g......;9;1;10;1,9,60,600,30,25,10,60,0
                        //9 ROOM 105(F): Mobile_AUSB;X;Mobile_AUSB;19;1;15
                        //9 ROOM 106(R): Mobile2;X;Mobile2;19;1;10
                        //9 ROOM 104(R): Mobile_test;X;Mobile_test;19;1;15
                        //9 ROOM 96(F): GNUGO Robot Room;X;...........;19;50;0;1,19,3000,0,0,0,0,0,0
                        //9 ROOM 103(F): Mobile;X;Mobile;19;1;15
                        //9 ROOM 88(R): HikariTVGame C;X;......TV.....b;19;10;15
                        //9 ROOM 89(R): HikariTVGame D;X;......TV.....c;19;10;15
                        //9 ROOM 27(R): SportsAccord;X;.X.|.[.c.A.R.[.h;19;10;15

                        var r = new Room();
                        var firstColon = cmd.Text.IndexOf(':');
                        r.Id = cmd.Text.Substring(5, firstColon - 1);
                        //var firstSemiColon = cmd.Text.Substring(firstColon + 2, firstSemiColon)
                    }
                    else if (cmd.Code == Codes.CODE_ERROR)
                    {
                        InvokeErrorEvent(cmd);
                        Disconnect();
                        return;
                    }
                    else
                    {
                        InvokeUnexpectedResponseEvent(cmd);
                        Disconnect();
                        return;
                    }
                    break;
                default:
                    break;
            }


        }

        /// <summary>
        /// If cmd.Code is CODE_INFO, pops items off command stack.  Otherwise fires an error or
        /// unexpected event and disconnects.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>True if no more commands left on the queue, false otherwise.</returns>
        private bool ProcessCommandQueue(Command cmd)
        {
            if (_commandQueue.Any())
            {
                if (cmd.Code == Codes.CODE_INFO)
                {
                    _commandQueue.RemoveAt(0);
                    if (!_commandQueue.Any())
                        return true;
                }
                else if (cmd.Code == Codes.CODE_ERROR)
                {
                    InvokeErrorEvent(cmd);
                    Disconnect();
                }
                else
                {
                    InvokeUnexpectedResponseEvent(cmd);
                    Disconnect();
                }
                return false;
            }
            else
            {
                var ex = new Exception("HandleSingleCommandResult() was called when the command queue was empty.  This should not happen.  See .Data for cmd and status.");
                ex.Data.Add("cmd", cmd);
                ex.Data.Add("Status", Status);
                throw ex;
            }
        }

        private async Task SendMultipleCommands(params string[] cmds)
        {
            _commandQueue.Clear();
            foreach (var c in cmds)
            {
                await SendMessageAsync(c);
                _commandQueue.Add(c);
            }
        }

        private bool _insideFile = false;

        private IEnumerable<Command> SplitMessageIntoCommands(string msg)
        {
            if (_savedPartial != null)
            {
                msg = _savedPartial + msg;
                _savedPartial = null;
            }

            var rval = new List<Command>(5000);

            // The "Login: " line doesn't have a code preceding it, so we do a special check
            // and give it a code.
            if (Status == States.Connected)
            {
                if (msg.EndsWith("\r\nLogin: "))
                {
                    rval.Add(new Command(Codes.CODE_PROMPT, SubCodes.STAT_LOGON, "Login: "));
                    return rval;
                }
            }

            var line = new StringBuilder(200);
            
            foreach (char c in msg)
            {
                if (c == '\r')
                {
                    // Optimization - don't parse or create new StringBuilder unless the original
                    // has data in it.
                    if (line.Length > 0)
                    {
                        Codes code;
                        SubCodes? subCode;
                        string text;
                        var l = line.ToString();
                        ParseLine(l, out code, out subCode, out text);

                        // Skip lines inside a "9 File" section.
                        if (code == Codes.CODE_INFO && text == "File")
                            _insideFile = !_insideFile;

                        if (!_insideFile)
                            rval.Add(new Command(code, subCode, text));
                        line = new StringBuilder(200);
                    }
                    else
                        rval.Add(new Command(Codes.CODE_NONE, null, null));
                }
                else if (c == '\n')
                {
                    // Discard; the \r is enough to call it a new line.
                }
                else
                    line.Append(c);
            }

            // Save partial line, process it next time.
            if (line.Length > 0)
                _savedPartial = line.ToString();

            return rval;
        }

        private void ParseLine(string line, out Codes code, out SubCodes? subCode, out string text)
        {
            // Optimization - test "1 5 " scenario first.
            if (line == "1 5")
            {
                code = Codes.CODE_PROMPT;
                subCode = SubCodes.STAT_WAITING;
                text = null;
                return;
            }

            // Shouts happen often, so check that right off too.
            if (line.StartsWith("21 "))
            {
                code = Codes.CODE_SHOUT;
                subCode = null;
                text = line.Substring(3);
                return;
            }

            // Check all the remaining codes.
            foreach (var key in LinePrefixes.Keys)
            {
                // No text.
                if (line == key.TrimEnd())
                {
                    var cmd = LinePrefixes[key];
                    code = cmd.Code;
                    subCode = cmd.SubCode;
                    text = null;
                    return;
                }

                // Some text.
                if (line.StartsWith(key))
                {
                    var cmd = LinePrefixes[key];
                    code = cmd.Code;
                    subCode = cmd.SubCode;
                    text = line.Substring(key.Length);
                    return;
                }
            }

            code = Codes.CODE_NONE;
            subCode = null;
            text = line;
        }

        #region Events

        public event Action DisconnectedEvent;
        internal void InvokeDisconnectedEvent()
        {
            var e = DisconnectedEvent;
            if (e != null)
                e();
        }

        public event Action<States, Command> UnexpectedResponseEvent;
        internal void InvokeUnexpectedResponseEvent(Command cmd)
        {
            var e = UnexpectedResponseEvent;
            if (e != null)
                e(Status, cmd);
        }

        public event Action ConnectedEvent;
        internal void InvokeConnectedEvent()
        {
            var e = ConnectedEvent;
            if (e != null)
                e();
        }

        public event Action CouldNotConnectEvent;
        internal void InvokeCouldNotConnectEvent()
        {
            var e = CouldNotConnectEvent;
            if (e != null)
                e();
        }

        public event Action LoggedInEvent;
        internal void InvokeLoggedInEvent()
        {
            var e = LoggedInEvent;
            if (e != null)
                e();
        }

        public event Action InvalidPasswordEvent;
        internal void InvokeInvalidPasswordEvent()
        {
            var e = InvalidPasswordEvent;
            if (e != null)
                e();
        }

        public event Action<States, Command> ErrorEvent;
        internal void InvokeErrorEvent(Command cmd)
        {
            var e = ErrorEvent;
            if (e != null)
                e(Status, cmd);
        }

        public event Action<IEnumerable<Room>> GotRoomsEvent;
        internal void InvokeGotRoomsEvent(IEnumerable<Room> rooms)
        {
            var e = GotRoomsEvent;
            if (e != null)
                e(rooms);
        }

        #endregion Events
        
        async private Task WaitForDataAsync()
        {
            // This method is only called by Connect().  It takes the connected socket and enters
            // a loop, exiting only when no data is detected for some time (a lost connection), 
            // or the connection is lost some other way.

            // Our cancellation token allows us to terminate the connection and exit early.
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            const uint bufferCapacity = 4*1024; // KB
            var buffer = new Windows.Storage.Streams.Buffer(bufferCapacity);  // KB

            try
            {
                while (true)
                {
                    // This will throw only if _cts.Cancel() was called by the Disconnect() method.
                    token.ThrowIfCancellationRequested();
                    
                    // ReadAsync has a built in disconnect timeout, so we don't have to implement
                    // a timer to check for a half open connection.  See 
                    // http://blog.stephencleary.com/2009/05/detection-of-half-open-dropped.html.
                    // Note that the data it reads doesn't necessarily contain full lines
                    // of data.  It could be split anywhere, so we must take that into account.
                    var readOperation = _socket.InputStream.ReadAsync(buffer, bufferCapacity, InputStreamOptions.Partial);
                    var result = await readOperation;
                    if (result.Length <= 0 || readOperation.Status != Windows.Foundation.AsyncStatus.Completed)
                    {
                        _cts.Cancel();
                        continue;
                    }
                    else
                    {
                        var msg = Encoding.UTF8.GetString(result.ToArray(), 0, (int)result.Length);
                        LogMessage(string.Format("Received (from {0}): {1}", _socket.Information.RemoteHostName.DisplayName, msg));
                        await UpdateStateMachine(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Status = States.ConnectionClosed;
                InvokeDisconnectedEvent();
            }
        }

        public void Disconnect()
        {
            Status = States.Disconnecting;
            _cts.Cancel();
        }

        public async Task Connect(string userid, string password)
        {
            try
            {
                try
                {
                    if (IsConnected)
                        throw new InvalidOperationException(
                            "Please close the connection before starting a new one, or use a separate instance.");

                    UserId = userid;
                    Password = password;

                    // This is necessary to reset the parser properly.
                    _savedPartial = null;
                    _insideFile = false;

                    _socket = new StreamSocket();
                    IsConnecting = true;
                    await _socket.ConnectAsync(_host, _port.ToString());
                    //, SocketProtectionLevel.SslAllowNullEncryption);
                    IsConnecting = false;
                    Status = States.Connected;
                    IsConnected = true;
                    InvokeConnectedEvent();

                    LogMessage(string.Format("Connected to {0}", _socket.Information.RemoteHostName.DisplayName));
                }
                catch (Exception ex)
                {
                    InvokeCouldNotConnectEvent();
                    return;
                }

                await WaitForDataAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (_socket != null)
                    _socket.Dispose();
                IsConnecting = false;
                IsConnected = false;
                _socket = null;
            }
        }

        private void LogMessage(string message)
        {
            Debug.WriteLine(message);
        }

        private async Task SendMessageAsync(string message)
        {
            if (!IsConnected)
                throw new Exception("Not connected, can't send message: " + message);

            var writer = new DataWriter(_socket.OutputStream);
            writer.WriteString(message);
            var ret = await writer.StoreAsync();
            writer.DetachStream();

            LogMessage(string.Format("Sent (to {0}) {1}", _socket.Information.RemoteHostName.DisplayName, message));
        }

        #endregion Private Methods

        #region Public Methods

        #endregion Public Methods

        #region IDisposable Members

        public void Dispose()
        {
            if (IsConnected)
                Disconnect();
        }

        #endregion
    }
}
