using System;
using System.Threading.Tasks;
using GoG.Infrastructure.ViewModels;
using NovaGS.WinRT.Model;
using SocketIOClient.WinRT;

namespace NovaGS.WinRT
{
    public class NovaGsClient : ViewModelBase, IDisposable
    {
        #region Data
        private readonly SocketIoClient _client;
        private int _currentPlayerId = 0;
        #endregion Data

        #region Ctor
        
        public NovaGsClient()
        {

            _client = new SocketIoClient("http://ggs.nova.gs/") { RetryConnectionAttempts = 0 };

            _client.Error += SocketIoOnError;
            _client.Opened += SocketIoOnOpened;
            _client.SocketConnectionClosed += SocketIoOnConnectionClosed;
            _client.ConnectionRetryAttempt += SocketIoOnConnectionRetryAttempt;

            SubscribeToSocketIoMessages();
        }

        private void SubscribeToSocketIoMessages()
        {
            _client.On("open",
                msg =>
                {

                });
        }
        
        #endregion Ctor

        #region Properties

        private string _lastError;
        public string LastError
        {
            get { return _lastError; }
            set { SetProperty(ref _lastError, value); }
        }

        private ConnectionState _connectionState = ConnectionState.Disconnected;
        public ConnectionState ConnectionState
        {
            get { return _connectionState; }
            set { SetProperty(ref _connectionState, value); }
        }

        #endregion Properties

        #region Events

        public event EventHandler Connected;
        protected virtual void OnConnected()
        {
            var handler = Connected;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler Disconnected;
        protected virtual void OnDisconnected()
        {
            var handler = Disconnected;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler Connecting;

        protected virtual void OnConnecting()
        {
            var handler = Connecting;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler<NovaConnectionError> ConnectionFailed;

        protected virtual void OnConnectionFailed(NovaConnectionError connectionError)
        {
            var handler = ConnectionFailed;
            if (handler != null) handler(this, connectionError);
        }

        public event EventHandler<GameData> GameDataUpdated;

        protected virtual void OnGameDataUpdated(GameData e)
        {
            EventHandler<GameData> handler = GameDataUpdated;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<GameClockUpdate> GameClockUpdated;

        protected virtual void OnGameClockUpdated(GameClockUpdate e)
        {
            EventHandler<GameClockUpdate> handler = GameClockUpdated;
            if (handler != null) handler(this, e);
        }

        #endregion Events

        #region Public Methods

        public async Task Connect(string userId, string password)
        {
            await _client.ConnectAsync(userId, password);
        }

        public void Disconnect()
        {
            _client.Close();
        }

        public void ConnectToGame(int gameId)
        {
            _client.On("gamedata " + gameId, msg =>
            {
                var gd = msg.Json.GetFirstArgAs<GameData>();
                OnGameDataUpdated(gd);
            });
            _client.On("setGameClock " + gameId, msg =>
            {
                var gcu = msg.Json.GetFirstArgAs<GameClockUpdate>();
                OnGameClockUpdated(gcu);        
            });
            _client.Emit("connectToGame", new {player_id = _currentPlayerId, game_id = gameId});
        }
        
        #endregion Public Methods

        #region Private Methods
        
        private void SocketIoOnConnectionRetryAttempt(object sender, EventArgs eventArgs)
        {
            ConnectionState = ConnectionState.Connecting;
            OnConnecting();
        }

        private void SocketIoOnConnectionClosed(object sender, EventArgs eventArgs)
        {
            ConnectionState = ConnectionState.Disconnected;
            OnDisconnected();
        }

        private void SocketIoOnOpened(object sender, EventArgs eventArgs)
        {
            ConnectionState = ConnectionState.Connected;
            OnConnected();
        }

        private void SocketIoOnError(object sender, ErrorEventArgs errorEventArgs)
        {
            LastError = errorEventArgs.Message;

            //OnConnectionFailed(NovaConnectionError.LoginFailed);
            //return;

            if (LastError.StartsWith("Error initializing handshake"))
            {
                OnConnectionFailed(NovaConnectionError.ConnectionFailed);
            }
            // Todo: figure out how to handle login errors.
            else if (LastError.StartsWith("Login failed"))
            {
                OnConnectionFailed(NovaConnectionError.LoginFailed);
            }
            else
            {
                OnConnectionFailed(NovaConnectionError.ConnectionFailed);
            }
        }

        #endregion Private Methods

        #region IDisposable Members

        public void Dispose()
        {
            _client.Error -= SocketIoOnError;
            _client.Opened -= SocketIoOnOpened;
            _client.SocketConnectionClosed -= SocketIoOnConnectionClosed;
            _client.ConnectionRetryAttempt -= SocketIoOnConnectionRetryAttempt;

            _client.Dispose();
        }

        #endregion
    }
}
