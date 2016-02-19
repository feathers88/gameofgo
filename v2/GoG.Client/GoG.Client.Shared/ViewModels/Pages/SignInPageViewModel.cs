using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using GoG.Client.Services;
using GoG.Client.Services.Popups;
using LiveSDKHelper;
using Microsoft.Live;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Unity;

namespace GoG.Client.ViewModels.Pages
{
    public interface ISignInPageViewModel : IPageViewModelBase
    {
        event EventHandler InvalidPasswordEvent;
    }

    public class SignInPageViewModel : ViewModelBase, ISignInPageViewModel
    {
        #region Data

        private readonly IGoGService _onlineGoService;
        private readonly IPopupService _popupService;
        private readonly INavigationService _navigationService;
        private readonly ILoginInstructionUserControlViewModel _loginInstructions;

        #endregion Data

        #region Ctor and Init

        public SignInPageViewModel()
        {
        }

        [InjectionConstructor]
        public SignInPageViewModel(
            IGoGService onlineGoService,
            IPopupService popupService, 
            INavigationService navigationService,
            ILoginInstructionUserControlViewModel loginInstructions)
        {
            _onlineGoService = onlineGoService;
            _popupService = popupService;
            _navigationService = navigationService;
            _loginInstructions = loginInstructions;

            if (_onlineGoService.SignedInUser != null)
            {
                _userName = _onlineGoService.SignedInUser.UserId;
                IsNewSignIn = false;
            }
            else
            {
                IsNewSignIn = true;
            }

            Title = "The Game of Go";
        }

        #endregion Ctor and Init

        #region Properties

        public string Title { get; private set; }

        #region CurrentUser
        public ICurrentUserUserControlViewModel CurrentUser { get { return null; } }
        #endregion CurrentUser

        #region UserName
        private string _userName;
        [RestorableState]
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (SetProperty(ref _userName, value))
                {
                    _signInCommand.RaiseCanExecuteChanged();
                }
            }
        }
        #endregion UserName

        #region Password
        private string _password;
        [RestorableState]
        public string Password
        {
            get { return _password; }
            set
            {
                if (SetProperty(ref _password, value))
                {
                    _signInCommand.RaiseCanExecuteChanged();
                }
            }
        }
        #endregion Password

        #region IsSignInValid
        private bool _isSignInInvalid;
        [RestorableState]
        public bool IsSignInInvalid
        {
            get { return _isSignInInvalid; }
            private set { SetProperty(ref _isSignInInvalid, value); }
        }

        public bool IsNewSignIn { get; set; }
        #endregion IsSignInValid

        #endregion Properties

        #region Events

        public event EventHandler InvalidPasswordEvent;

        #endregion Events

        #region Commands

        #region SignInCommand
        DelegateCommand _signInCommand;
        public ICommand SignInCommand
        {
            get { return _signInCommand ?? (_signInCommand = DelegateCommand.FromAsyncHandler(ExecuteSignInCommand, CanSignInCommand)); }
        }
        public bool CanSignInCommand()
        {
            return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password) && !IsBusy;
        }
        public async Task ExecuteSignInCommand()
        {
            try
            {
                IsBusy = true;

                var signinCallFailed = false;
                var signinSuccessfull = false;
                try
                {
                    signinSuccessfull = await _onlineGoService.SignInUserAsync(UserName, Password, true);
                }
                catch (WebException)
                {
                    signinCallFailed = true;
                }
                if (signinCallFailed)
                {
                    //await _popupService.ShowAsync(_resourceLoader.GetString(ResourceStringKeys.ErrorServiceUnreachable), _resourceLoader.GetString(ResourceStringKeys.Error));
                    InvalidPasswordEvent(null, null);
                    return;
                }
                if (signinSuccessfull)
                {
                    IsSignInInvalid = false;

                    _navigationService.GoBack();
                }
                else
                {
                    IsSignInInvalid = true;
                    InvalidPasswordEvent(null, null);
                    //await _popupService.ShowAsync("Invalid user id or application password, or you're not connected to the Internet.", "Incorrect");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion SignInCommand

        #region WhatIsThisCommand
        DelegateCommand _whatIsThisCommand;
        public DelegateCommand WhatIsThisCommand
        {
            get { if (_whatIsThisCommand == null) _whatIsThisCommand = DelegateCommand.FromAsyncHandler(ExecuteWhatIsThisCommand, CanWhatIsThisCommand); return _whatIsThisCommand; }
        }
        public bool CanWhatIsThisCommand()
        {
            return true;
        }
        public async Task ExecuteWhatIsThisCommand()
        {
            await _popupService.ShowAsync(_loginInstructions);
        }
        #endregion WhatIsThisCommand

        #region GoBackCommand
        DelegateCommand _goBackCommand;
        public ICommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new DelegateCommand(ExecuteGoBackCommand, CanGoBackCommand)); }
        }
        public bool CanGoBackCommand()
        {
            return false;
        }
        public void ExecuteGoBackCommand()
        {

        }
        #endregion GoBackCommand

        #endregion Commands

        #region Virtuals

        /// <summary>
        /// Presents a UI used to sign in.  User cannot go 'back' manually.
        /// That is done automatically after login is successful.
        /// </summary>
        /// <param name="navigationParameter"></param>
        /// <param name="navigationMode"></param>
        /// <param name="viewModelState"></param>
        public void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode,
            Dictionary<string, object> viewModelState)
        {
            Task.Factory.StartNew(
                async () =>
                {
                    var sessionDetails = await LiveLoginAsync();
                    while (sessionDetails == null)
                        sessionDetails = await LiveLoginAsync();
                    
                });

        }
        
        public void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
        }

        #endregion Virtuals

        #region Private Helpers

        
        
        #endregion Private Helpers

        #region IPageViewModelBase implementation
        
        #endregion IPageViewModelBase implementation

    }
}
