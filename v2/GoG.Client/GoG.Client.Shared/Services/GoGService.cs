using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GoG.Client.Exceptions;
using GoG.Client.Models;
using GoG.Client.Models.OnlineGoService.GameDetails;
using GoG.Client.Models.OnlineGoService.MyGames;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Newtonsoft.Json;
using WinRTXamlToolkit.Async;

namespace GoG.Client.Services
{
    /// <summary>
    /// Handles our connection and communication.  Uses events the view model can use to 
    /// display what's going on.
    /// </summary>
    public interface IGoGService
    {
        bool IsConnected { get; }

        event EventHandler Connected;
        event EventHandler Disconnected;
        
        /// <summary>
        /// Gets an object that contains some of the games, and a url for the next and previous pages.
        /// </summary>
        /// <returns></returns>
        Task<GamesPager> GetGamesPagerAsync(int pageNo);

        Task<GameDetails> GetGameDetailsAsync(int gameId);
    }

    public class GoGService : IGoGService
    {
        #region Data

        private readonly Settings _settings;

        private readonly object _raisingIsConnectedChangedLock = new object();

        private static readonly AsyncLock ConnectLock = new AsyncLock();
        
        private readonly ISessionStateService _sessionStateService;
        private readonly ILoginManager _loginManager;

        #endregion Data

        #region Ctor and Init

        public GoGService(
            ISessionStateService sessionStateService,
            ILoginManager loginManager,
            IConfigurationService configurationService)
        {
            _sessionStateService = sessionStateService;
            _loginManager = loginManager;
            _settings = configurationService.Settings;
        }

        #endregion Ctor and Init

        #region Events

        public event EventHandler Connected;
        public event EventHandler Disconnected;

        #endregion Events

        #region Properties

        #region Session
        public Session Session { get; private set; }
        #endregion Session

        #region IsConnected
        public bool IsConnected
        {
            get { return Session != null; }
        }
        #endregion IsConnected

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Logs in.  Fires Connected and Disconnected events as necessary.
        /// </summary>
        /// <returns>Success</returns>
        public async Task ConnectAsync()
        {
            // Formally disconnect if not already done so.
            if (Session != null)
            {
                Session = null;
                RaiseDisconnected();
            }

            try
            {
                // Attempt reconnect.
                Session = await _loginManager.LoginAsync();
                if (Session != null)
                    RaiseConnected();
            }
            catch (UnauthorizedException)
            {
                throw;
            }
            catch (HttpErrorCodeException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<GamesPager> GetGamesPagerAsync(int pageNo)
        {
            var rval = await Get<GamesPager>(String.Format(_settings.GamesPagerUri, pageNo));
            return rval;
        }

        public async Task<GameDetails> GetGameDetailsAsync(int gameId)
        {
            var rval = await Get<GameDetails>(String.Format(_settings.GameDetailsUri, gameId));
            return rval;
        }

        #endregion Public Methods

        #region Private Helpers

        private void SetAuthHeader(HttpRequestMessage msg)
        {
            if (IsConnected)
                msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Session.AuthenticationToken);
        }

        /// <summary>
        /// Given the model type and the uri, gets the object from the server.  If unauthorized, we try to login and
        /// raise the Disconnected event if unsuccessful.  Returns null on failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        private async Task<T> Get<T>(string uri) where T : class
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(uri),
                    Method = HttpMethod.Get
                };
                SetAuthHeader(request);

                for (int i = 0; i < 3; i++)
                {
                    var result = await client.SendAsync(request);

                    if (result.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        // Attempt to login.
                        Session = await _loginManager.LoginAsync();
                        if (!IsConnected)
                        {
                            Session = null;
                            RaiseDisconnected();
                            return null;
                        }
                        continue;
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        var str = await result.Content.ReadAsStringAsync();
                        var rootObject = JsonConvert.DeserializeObject<T>(str);

                        return rootObject;
                    }
                }
            }

            return null;
        }

        private void RaiseConnected()
        {
            lock (_raisingIsConnectedChangedLock)
            {
                var e = Connected;
                if (e != null)
                    e(this, null);
            }
        }

        private void RaiseDisconnected()
        {
            lock (_raisingIsConnectedChangedLock)
            {
                var e = Disconnected;
                if (e != null)
                    e(this, null);
            }
        }

        #endregion Private Helpers
    }
}
