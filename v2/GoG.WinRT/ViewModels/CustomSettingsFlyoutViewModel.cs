using System;
using System.Threading.Tasks;
using GoG.WinRT.Common;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using NovaGS.WinRT;

namespace GoG.WinRT.ViewModels
{
    public class CustomSettingsFlyoutViewModel : BaseViewModel, IFlyoutViewModel
    {
        #region Data
        private IUnityContainer _container;
        #endregion Data

        #region Ctor
        public CustomSettingsFlyoutViewModel(IUnityContainer c)
        {
            _container = c;
        }
        #endregion Ctor
        
        #region Properties

        #region DisplayNovaUserId
        private string _displayNovaUserId;
        public string DisplayNovaUserId
        {
            get { return _displayNovaUserId; }
            set
            {
                if (SetProperty(ref _displayNovaUserId, value))
                {
                    AuthenticateNovaGSCredentialsCommand.RaiseCanExecuteChanged();
                    BusyMessage = null;
                }
            }
        }
        #endregion DisplayNovaUserId

        #region DisplayNovaPassword
        private string _displayNovaPassword;
        public string DisplayNovaPassword
        {
            get { return _displayNovaPassword; }
            set
            {
                if (SetProperty(ref _displayNovaPassword, value))
                {
                    AuthenticateNovaGSCredentialsCommand.RaiseCanExecuteChanged();
                    BusyMessage = null;
                }
            }
        }
        #endregion DisplayNovaPassword

        #region NovaGSUserId
        private string _novaGSUserId;
        public string NovaGSUserId { get { return _novaGSUserId; } }
        #endregion NovaGSUserId

        #region NovaGSPassword
        private string _novaGSPassword;
        public string NovaGSPassword { get { return _novaGSPassword; } }
        #endregion NovaGSPassword

        #region IsBusy
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }
        #endregion IsBusy

        #region BusyMessage
        private string _busyMessage;
        public string BusyMessage
        {
            get { return _busyMessage; }
            set { SetProperty(ref _busyMessage, value); }
        }
        #endregion BusyMessage

        #endregion Properties

        #region Commands

        #region AuthenticateNovaGSCredentialsCommand
        DelegateCommand _authenticateNovaGSCredentialsCommand;
        
        public DelegateCommand AuthenticateNovaGSCredentialsCommand
        {
            get { return _authenticateNovaGSCredentialsCommand ?? (_authenticateNovaGSCredentialsCommand = new DelegateCommand(ExecuteAuthenticateNovaGSCredentials, CanAuthenticateNovaGSCredentials)); }
        }
        public bool CanAuthenticateNovaGSCredentials()
        {
            if (IsBusy)
                return false;
            if (String.IsNullOrWhiteSpace(DisplayNovaUserId) && String.IsNullOrWhiteSpace(DisplayNovaPassword))
                return true;
            return !String.IsNullOrWhiteSpace(DisplayNovaUserId) && !String.IsNullOrWhiteSpace(DisplayNovaPassword);
        }
        public async void ExecuteAuthenticateNovaGSCredentials()
        {
            // Test the user entered credentials against the server.
            BusyMessage = null;
            IsBusy = true;
            AuthenticateNovaGSCredentialsCommand.RaiseCanExecuteChanged();
            var success = await ConnectToNovaUsingDisplayCredentials();
            IsBusy = false;
            AuthenticateNovaGSCredentialsCommand.RaiseCanExecuteChanged();

            if (success)
            {
                // Save displayed credentials to store and close flyout.
                if (String.IsNullOrEmpty(DisplayNovaUserId))
                    StorageHelper.SaveCredentials("NovaGSCredentials", "anonymous", "anonymous");
                else
                    StorageHelper.SaveCredentials("NovaGSCredentials", DisplayNovaUserId.Trim(), DisplayNovaPassword.Trim());
                _novaGSUserId = DisplayNovaUserId;
                _novaGSPassword = DisplayNovaPassword;
                this.CloseFlyout();
            }
            else
            {
                BusyMessage = "Invalid username or password.";
            }
        }

        #endregion AuthenticateNovaGSCredentialsCommand

        #endregion Commands

        #region Private

        private async Task<bool> ConnectToNovaUsingDisplayCredentials()
        {
            var client = new NovaGSClient();
            try
            {
                await client.Connect(DisplayNovaUserId, DisplayNovaPassword);
                if (client.ConnectionState == ConnectionState.Connected)
                    return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                try
                {
                    client.Disconnect();
                }
                catch
                {
                }
            }
            return false;
        }

        #endregion Private

        #region Public Methods
        public void GetCredentials()
        {
            StorageHelper.GetCredentials("NovaGSCredentials", out _novaGSUserId, out _novaGSPassword);
            if (_novaGSUserId == "anonymous")
            {
                _novaGSUserId = String.Empty;
                _novaGSPassword = String.Empty;
            }
        }
        #endregion Public Methods

        #region IFlyoutViewModel Members

        public Action CloseFlyout { get; set; }
        
        public Action GoBack { get; set; }
        
        public void Open(object parameter, Action successAction)
        {
            BusyMessage = null;
            GetCredentials();
            DisplayNovaUserId = NovaGSUserId;
            DisplayNovaPassword = NovaGSPassword;
        }

        #endregion IFlyoutViewModel Members
    }
}
