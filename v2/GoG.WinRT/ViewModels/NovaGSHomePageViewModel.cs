using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using GoG.WinRT.Views;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using NovaGS.WinRT;
using NovaGS.WinRT.Model;

namespace GoG.WinRT.ViewModels
{
    public class NovaGSHomePageViewModel : PageViewModel
    {
        #region Data
        private IFlyoutService _flyoutService;
        private int _nonLoginConnectionErrors;
        #endregion Data

        #region Ctor
        public NovaGSHomePageViewModel(IUnityContainer c, NovaGSClient novaGSClient,
            CustomSettingsFlyoutViewModel customSettingsFlyout,
            IFlyoutService flyoutService)
            : base(c)
        {
            Client = novaGSClient;
            WireUpClientEvents();

            CustomSettings = customSettingsFlyout;
            _flyoutService = flyoutService;
        }

        private void WireUpClientEvents()
        {
            Client.Connected -= ClientOnConnected;
            Client.Disconnected -= ClientOnDisconnected;
            Client.Connecting -= ClientOnConnecting;
            Client.ConnectionFailed -= ClientOnConnectionFailed;
            Client.GameDataUpdated -= ClientOnGameDataUpdated;
            Client.GameClockUpdated -= ClientOnGameClockUpdated;

            Client.Connected += ClientOnConnected;
            Client.Disconnected += ClientOnDisconnected;
            Client.Connecting += ClientOnConnecting;
            Client.ConnectionFailed += ClientOnConnectionFailed;
            Client.GameDataUpdated += ClientOnGameDataUpdated;
            Client.GameClockUpdated += ClientOnGameClockUpdated;
        }

        private void ClientOnGameClockUpdated(object sender, GameClockUpdate gameClockUpdate)
        {
                
        }

        private void ClientOnGameDataUpdated(object sender, GameData gameData)
        {
            
        }

        private async void ClientOnConnectionFailed(object sender, NovaConnectionError novaConnectionError)
        {
            this.RunOnUIThread(async () =>
            {

                switch (novaConnectionError)
                {
                    case NovaConnectionError.ConnectionFailed:
                        IsBusy = true;
                        BusyMessage = "Connecting...";
                        await Task.Delay(1000);
                        Connect();
                        break;
                    case NovaConnectionError.LoginFailed:
                        IsBusy = false;
                        BusyMessage = null;
                        Debug.WriteLine("Login failed.  Opening settings...");
                        await
                            this.DisplayMessage("Login Failure",
                                "Your login credentials were rejected.  Please correct, or delete your Nova.GS User Id and Password to log in anonymously.\n\nSettings will now open.");
                        await Task.Delay(200);
                        _flyoutService.ShowFlyout("CustomSettings");
                        break;
                    case NovaConnectionError.None:
                        Debug.WriteLine("Shouldn't get here.");
                        break;
                }
            });
        }

        private void ClientOnConnecting(object sender, EventArgs eventArgs)
        {
            IsBusy = true;
            BusyMessage = "Connecting...";
        }

        private async void ClientOnDisconnected(object sender, EventArgs eventArgs)
        {
            while (!AbortOperation && Client.ConnectionState != NovaGS.WinRT.ConnectionState.Connected)
            {
                Connect();
            }
        }

        private void Connect()
        {
            // Get credentials from store just in case they've just been changed (even on another device).
            CustomSettings.GetCredentials();
            Client.Connect(CustomSettings.NovaGSUserId, CustomSettings.NovaGSPassword);
        }

        private void ClientOnConnected(object sender, EventArgs eventArgs)
        {
            IsBusy = false;
            BusyMessage = null;

            Client.ConnectToGame(39);
        }

        #endregion Ctor

        #region Properties

        #region CurrentUserId
        private string _currentUserId;
        public string CurrentUserId
        {
            get { return _currentUserId; }
            set { if (SetProperty(ref _currentUserId, value)) OnPropertyChanged("CurrentIsAnonymous"); }
        }
        #endregion CurrentUserId

        #region CurrentPassword
        private string _currentPassword;
        public string CurrentPassword
        {
            get { return _currentPassword; }
            set { SetProperty(ref _currentPassword, value); }
        }
        #endregion CurrentPassword

        public bool CurrentIsAnonymous { get { return String.IsNullOrWhiteSpace(CurrentUserId); } }

        public NovaGSClient Client { get; private set; }
        public CustomSettingsFlyoutViewModel CustomSettings { get; private set; }

        //#region ConnectionState
        //private string _connectionState;
        //public string ConnectionState
        //{
        //    get { return _connectionState; }
        //    set { SetProperty(ref _connectionState, value); }
        //}
        //#endregion ConnectionState

        #endregion Properties

        #region Virtuals

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);

            IsBusy = true;
            BusyMessage = "Connecting...";
            Connect();
        }

        #endregion Virtuals
    }
}
