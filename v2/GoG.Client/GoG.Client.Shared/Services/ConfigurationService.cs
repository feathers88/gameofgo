// ReSharper disable CheckNamespace

namespace GoG.Client.Services
{
    public class Settings
    {
        public Settings(string apiRootUri)
        {
            MicrosoftTokenGeneratorUri = "https://login.live.com/oauth20_authorize.srf";

            // The rest are passed in or relative to passed in values.  This
            // retains control outside this class.
            ApiUriRootUri = apiRootUri;
            ProfileUri = ApiUriRootUri + "me/profile/";
            GamesPagerUri = ApiUriRootUri + "me/games/?page={0}";
            GameDetailsUri = ApiUriRootUri + "games/{0}";
            MyUserDetailsUri = ApiUriRootUri + "me/";
            MyUserSettingsUri = ApiUriRootUri + "me/settings/";
        }

        // Root of API all uris.
        public string ApiUriRootUri { get; private set; }
        
        public string ProfileUri { get; private set; }

        public string MicrosoftTokenGeneratorUri { get; private set; }
        
        /// <summary>
        /// Takes parameter, the page #.
        /// </summary>
        public string GamesPagerUri { get; private set; }
        /// <summary>
        /// Takes parameter, the game id.
        /// </summary>
        public string GameDetailsUri { get; private set; }
        public string MyUserDetailsUri { get; private set; }
        public string MyUserSettingsUri { get; private set; }
    }

    public interface IConfigurationService
    {
        Settings Settings { get; }
    }

    /// <summary>
    /// Manages the configuration stored in simple text files containing Key=Value pairs.  Old school. :)
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        #region Constants

        private const string ApiRootUri = "http://localhost:9999/";

        #endregion Constants

        #region Data
        #endregion Data

        #region Ctor and Init

        public ConfigurationService()
        {
            _settings = new Settings(ApiRootUri);
        }
        
        #endregion Ctor and Init

        #region IConfigurationService Members

        private readonly Settings _settings;
        public Settings Settings
        {
            get { return _settings; }
        }

        #endregion IConfigurationService Members

        #region Private Helpers
        
        #endregion Private Helpers
    }
}
