using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Navigation;
using GoG.Client.Models;
using GoG.Client.Services;
using GoG.Client.ViewModels.Pages;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;

namespace GoG.Client.ViewModels
{
    public enum PageAreas
    {
        MyGames
    }

    public class PageViewModelBase : ViewModel, IPageViewModelBase
    {
        #region Data

        private const string CurrentUserIdKey = "CurrentUserId";
        private const string CurrentUserGravatarKey = "CurrentUserGravatar";

        protected readonly IGoGService OnlineGoService;
        protected readonly ISessionStateService SessionStateService;

        protected readonly INavigationService NavigationService;

        #endregion Data

        #region Ctor and Init

        public PageViewModelBase(
            INavigationService navigationService,
            IGoGService onlineGoService,
            ISessionStateService sessionStateService)
        {
            if (DesignMode.DesignModeEnabled)
                return;

            OnlineGoService = onlineGoService;
            SessionStateService = sessionStateService;
            NavigationService = navigationService;

            _currentUser = new CurrentUserUserControlViewModel();

            OnlineGoService.LoggedIn += GoServiceOnUserChanged;

            _title = "The Game of Go";
        }

        #endregion Ctor and Init

        #region Properties

        #region CurrentPageArea
        private PageAreas _currentPageArea;
        public PageAreas CurrentPageArea
        {
            get { return _currentPageArea; }
            set { SetProperty(ref _currentPageArea, value); }
        }
        #endregion CurrentPageArea

        #region IsBusy

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        #endregion IsBusy


        #region IsConnecting
        private bool _isConnecting;
        public bool IsConnecting
        {
            get { return _isConnecting; }
            set { SetProperty(ref _isConnecting, value); }
        }
        #endregion IsConnecting

        #region CurrentUser
        private ICurrentUserUserControlViewModel _currentUser;
        public ICurrentUserUserControlViewModel CurrentUser
        {
            get { return _currentUser; }
            set { SetProperty(ref _currentUser, value); }
        }
        #endregion CurrentUser

        #region Title
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        #endregion Title

        #endregion Properties

        #region Commands

        #region GoBackCommand
        DelegateCommand _goBackCommand;
        public ICommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = DelegateCommand.FromAsyncHandler(ExecuteGoBackCommand, CanGoBackCommand)); } 
        }
        public bool CanGoBackCommand()
        {
            var canGoBack = OnCanGoBack();
            return canGoBack;
        }
        public async Task ExecuteGoBackCommand()
        {
            await OnGoBackPressedAsync();
        }
        #endregion GoBackCommand

        
        #region MyGamesCommand
        DelegateCommand _myGamesCommand;
        public DelegateCommand MyGamesCommand
        {
            get { if (_myGamesCommand == null) _myGamesCommand = new DelegateCommand(ExecuteMyGamesCommand, CanMyGamesCommand); return _myGamesCommand; }
        }
        public bool CanMyGamesCommand()
        {
            return true;
        }
        public void ExecuteMyGamesCommand()
        {
            NavigateAsync<IMyGamesPageViewModel>();
        }
        #endregion MyGamesCommand
      

        #endregion Commands

        #region Virtuals

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);

            // Restore CurrentUser from session state.
            object userId;
            if (SessionStateService.SessionState.TryGetValue(CurrentUserIdKey, out userId))
            {
                CurrentUser = new CurrentUserUserControlViewModel
                {
                    UserId = userId as string,
                    Gravatar = SessionStateService.SessionState[CurrentUserGravatarKey] as string
                };
            }

            OnlineGoService.IsConnectedChanged += GoServiceOnIsConnectedChanged;

            if (!OnlineGoService.IsConnected || CurrentUser == null)
            {
                // This happens asyncronously.
                ConnectOrSignIn();
            }
        }

        public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
            base.OnNavigatedFrom(viewModelState, suspending);

            // GoServiceOnIsConnectedChanged() causes navigation to the login page if
            // connection is lost.  So we don't want to handle it unless we're
            // the current page.
            OnlineGoService.IsConnectedChanged -= GoServiceOnIsConnectedChanged;
        }

        private async void ConnectOrSignIn()
        {
            // Try to connect if necessary, the forward to sign in page if we fail.
            if (OnlineGoService.IsConnected)
                return;

            try
            {
                IsConnecting = true;
                var connected = await OnlineGoService.ConnectAsync();
                if (!connected)
                    await NavigateAsync<ISignInPageViewModel>();
            }
            finally
            {
                IsConnecting = false;
            }
        }

        protected virtual bool OnCanGoBack()
        {
            return NavigationService.CanGoBack();
        }

        protected virtual void RaiseGoBackChanged()
        {
            _goBackCommand.RaiseCanExecuteChanged();
        }

        protected virtual Task OnGoBackPressedAsync()
        {
            NavigationService.GoBack();
            return Task.FromResult((object)null);
        }

        protected virtual void OnUserChanged()
        {
        }

        protected virtual void OnConnectedChanged()
        {
        }

        #endregion Virtuals

        #region Private Helpers and Event Handlers

        private void GoServiceOnUserChanged(object sender, UserChangedEventArgs userChangedEventArgs)
        {
            var user = new CurrentUserUserControlViewModel();
            if (userChangedEventArgs.NewUserInfo != null)
            {
                // Copy model into view model for display.
                user.UserId = userChangedEventArgs.NewUserInfo.UserId;
                if (!String.IsNullOrWhiteSpace(userChangedEventArgs.NewUserInfo.Settings.profile.icon))
                    user.Gravatar = userChangedEventArgs.NewUserInfo.Settings.profile.icon;
                else
                    user.Gravatar = "/Assets/appbar.futurama.bender30x52.png";

                SessionStateService.SessionState[CurrentUserIdKey] = user.UserId;
                SessionStateService.SessionState[CurrentUserGravatarKey] = user.Gravatar;
            }
            else
                NavigationService.Navigate<ISignInPageViewModel>();
            CurrentUser = user;

            OnUserChanged();
        }

        private async void GoServiceOnIsConnectedChanged(object sender, IsConnectedChangedEventArgs eventArgs)
        {
            if (!eventArgs.NewIsConnected)
            {
                while (true)
                {
                    try
                    {
                        IsConnecting = true;

                        await Task.Delay(1500);
                        await OnlineGoService.ConnectAsync();
                        if (OnlineGoService.IsConnected)
                            break;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        IsConnecting = false;
                    }
                }
            }

            OnConnectedChanged();
        }

        #endregion Private Helpers and Event Handlers

        #region Protected Methods

        protected Task<bool> NavigateAsync<TPageViewModel>(object parameter = null)
            where TPageViewModel : IPageViewModelBase
        {
            var tcs = new TaskCompletionSource<bool>();

            var sc = SynchronizationContext.Current;
            sc.Post(o =>
            {
                try
                {
                    var success = NavigationService.Navigate<TPageViewModel>(parameter);
                    tcs.SetResult(success);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                    throw;
                }
            }, null);

            return tcs.Task;
        }

        #endregion Protected Methods
    }
}
