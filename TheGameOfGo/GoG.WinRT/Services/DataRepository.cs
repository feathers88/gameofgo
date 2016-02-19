using System;
using System.ServiceModel;
using System.Threading.Tasks;
using GoG.Infrastructure.Engine;
using GoG.Infrastructure.Services.Engine;
using GoG.WinRT.FuegoService;
using GoG.WinRT.MessengerService;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;

namespace GoG.WinRT.Services
{
    public class DataRepository : IDataRepository
    {
        #region Data
        readonly ISessionStateService _sessionStateService;
        #endregion Data

        #region Ctor
        public DataRepository(ISessionStateService sessionStateService)
        {
            _sessionStateService = sessionStateService;
        }
        #endregion Ctor
        
        #region Fuego Implementation

        public async Task<GoResponse> GetGameExists(Guid gameid)
        {
            try
            {
                var c = GetFuegoClient();
                return await c.GetGameExistsAsync(gameid);
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoResponse(GoResultCode.CommunicationError);
            }
        }

        public async Task<GoGameStateResponse> GetGameStateAsync(Guid gameid)
        {
            try
            {
                var c = GetFuegoClient();
                return await c.GetGameStateAsync(gameid);
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoGameStateResponse(GoResultCode.CommunicationError, null);
            }
        }

        public async Task<GoGameStateResponse> StartAsync(Guid gameid, GoGameState state)
        {
            try
            {
                var c = GetFuegoClient();
                return await c.StartAsync(gameid, state);
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoGameStateResponse(GoResultCode.CommunicationError, null);
            }
        }

        //public async Task<GoResponse> PlaceAsync(Guid gameid, GoGameState clientState, ObservableCollection<GoMove> moves)
        //{
        //    var c = GetFuegoClient();
        //    return await c.PlaceAsync(gameid, clientState, moves);
        //}

        public async Task<GoMoveResponse> GenMoveAsync(Guid gameid, GoColor color)
        {
            try
            {
                var c = GetFuegoClient();
                return await c.GenMoveAsync(gameid, color);
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoMoveResponse(GoResultCode.CommunicationError, null, null);
            }
            
        }

        public async Task<GoMoveResponse> PlayAsync(Guid gameid, GoMove move)
        {
            try
            {
                var c = GetFuegoClient();
                return await c.PlayAsync(gameid, move);
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoMoveResponse(GoResultCode.CommunicationError, null, null);
            }
        }

        public async Task<GoHintResponse> HintAsync(Guid gameid, GoColor color)
        {
            try
            {
                var c = GetFuegoClient();
                return await c.HintAsync(gameid, color);
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoHintResponse(GoResultCode.CommunicationError, null);
            }
        }

        public async Task<GoGameStateResponse> UndoAsync(Guid gameid)
        {
            try
            {
                var c = GetFuegoClient();
                return await c.UndoAsync(gameid);
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoGameStateResponse(GoResultCode.CommunicationError, null);
            }
        }

        public async Task<GoSaveSVGResponse> SaveSGF(Guid gameid)
        {
            try
            {
                var c = GetFuegoClient();
                var resp = await c.SaveSGFAsync(gameid);
                return resp;
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoSaveSVGResponse(GoResultCode.CommunicationError, null);
            }
        }

        public async Task<GoResponse> LoadSGF(Guid gameid, string sgf)
        {
            try
            {
                var c = GetFuegoClient();
                var resp = await c.LoadSGFAsync(gameid, sgf);
                return resp;
            }
            catch
            {
                // Any kind of error is assumed to be internet connectivity.
                return new GoSaveSVGResponse(GoResultCode.CommunicationError, null);
            }
        }

        //public async Task<GoResponse> UndoAsync(Guid gameid, GoGameState clientState)
        //{
        //    var c = GetFuegoClient();
        //    return await c.UndoAsync(gameid, clientState);
        //}

        //public async Task<GoResponse> QuitAsync(Guid gameid)
        //{
        //    var c = GetFuegoClient();
        //    return await c.QuitAsync(gameid);
        //}

        //public async Task<string> NameAsync()
        //{
        //    var c = GetFuegoClient();
        //    return await c.NameAsync();
        //}

        //public async Task<string> VersionAsync()
        //{
        //    var c = GetFuegoClient();
        //    return await c.VersionAsync();
        //}

        //public async Task<GoScoreResponse> ScoreAsync(Guid gameid, GoGameState clientState)
        //{
        //    var c = GetFuegoClient();
        //    return await c.ScoreAsync(gameid, clientState);
        //}

        //public async Task<GoPositionsResponse> DeadAsync(Guid gameid, GoGameState clientState)
        //{
        //    var c = GetFuegoClient();
        //    return await c.DeadAsync(gameid, clientState);
        //}

        //public async Task<GoPositionsResponse> TerritoryAsync(Guid gameid, GoGameState clientState, GoColor color)
        //{
        //    var c = GetFuegoClient();
        //    return await c.TerritoryAsync(gameid, clientState, color);
        //}

        
        #endregion Fuego Implementation

        #region Chat async Implementation
        //public void SendMessage(string message)
        //{
        //    var c = GetChatClient();
        //    c.SendMessageAsync(message);
        //}
        #endregion Chat async Implementation

        #region Messenger Implementation

        public async Task<string> GetActiveMessage()
        {
            try
            {
                var c = GetMessengerClient();
                return await c.GetActiveMessageAsync();
            }
            catch
            {
                // Active message is not very important.  Just eat error.
                return null;
            }   
        }

        #endregion Messenger Implementation

        #region Private Helpers

        private FuegoServiceClient GetFuegoClient()
        {
            var binding = new BasicHttpBinding();
            binding.MaxBufferSize = int.MaxValue;
            binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.AllowCookies = true;

            // On developement machines, a HOSTS file entry should be created to point cbordeman.dnsalias.com to 127.0.0.1
            // for debugging purposes.
            var client = new FuegoServiceClient(binding, new EndpointAddress("http://cbordeman.dnsalias.com/GoG.Services/fuego.svc"));

            // Lots of time for debugging purposes.
#if DEBUG
            client.Endpoint.Binding.CloseTimeout = new TimeSpan(1, 0, 0, 0);
            client.Endpoint.Binding.ReceiveTimeout = new TimeSpan(1, 0, 0, 0);
            client.Endpoint.Binding.SendTimeout = new TimeSpan(1, 0, 0, 0);
#else
            client.Endpoint.Binding.CloseTimeout = new TimeSpan(0, 0, 0, 45);
            client.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 0, 0, 45);
            client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 0, 0, 45);
#endif
            return client;
        }

        private MessengerClient GetMessengerClient()
        {
            var binding = new BasicHttpBinding();
            binding.MaxBufferSize = int.MaxValue;
            binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.AllowCookies = true;

            // On developement machines, a HOSTS file entry should be created to point cbordeman.dnsalias.com to 127.0.0.1
            // for debugging purposes.
            var client = new MessengerClient(binding, new EndpointAddress("http://cbordeman.dnsalias.com/GoG.Services/messenger.svc"));

            // Lots of time for debugging purposes.
#if DEBUG
            client.Endpoint.Binding.CloseTimeout = new TimeSpan(1, 0, 0, 0);
            client.Endpoint.Binding.ReceiveTimeout = new TimeSpan(1, 0, 0, 0);
            client.Endpoint.Binding.SendTimeout = new TimeSpan(1, 0, 0, 0);
#else
            client.Endpoint.Binding.CloseTimeout = new TimeSpan(0, 0, 0, 5);
            client.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 0, 0, 5);
            client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 0, 0, 5);
#endif
            return client;
        }

        //private ChatClient GetChatClient()
        //{
        //    var client = new ChatClient();

        //    // Lots of time for debugging purposes.
        //    client.Endpoint.Binding.CloseTimeout = new TimeSpan(1, 0, 0, 0);
        //    client.Endpoint.Binding.ReceiveTimeout = new TimeSpan(1, 0, 0, 0);
        //    client.Endpoint.Binding.SendTimeout = new TimeSpan(1, 0, 0, 0);
        //    return client;
        //}

        #endregion Private Helpers
    }
}
