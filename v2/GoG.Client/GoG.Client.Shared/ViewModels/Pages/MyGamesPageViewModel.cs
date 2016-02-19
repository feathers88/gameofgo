using System.Threading.Tasks;
using GoG.Client.Services;
using GoG.Client.ViewModels.Games;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm.Interfaces;

// ReSharper disable once CheckNamespace
namespace GoG.Client.ViewModels.Pages
{
    public interface IMyGamesPageViewModel : IPageViewModelBase
    {
        Task Load();
        IncrementalObservableGameViewModelCollection GamesIncrementalLoader { get; }
    }

    public class MyGamesPageViewModel : PageViewModelBase, IMyGamesPageViewModel
    {
        #region Data

        #endregion Data

        #region Ctor and Init

        public MyGamesPageViewModel()
            : base(null, null, null)
        {
        }

        public MyGamesPageViewModel(INavigationService navigationService,
            IGoGService onlineGoService, ISessionStateService sessionStateService)
            : base(navigationService, onlineGoService, sessionStateService)
        {
            _gamesIncrementalLoader = new IncrementalObservableGameViewModelCollection(onlineGoService);

            CurrentPageArea = PageAreas.MyGames;
        }

        #endregion Ctor and Init

        #region Properties

        #region IsConnected
        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set { SetProperty(ref _isConnected, value); }
        }
        #endregion IsConnected

        #region Games
        private IncrementalObservableGameViewModelCollection _gamesIncrementalLoader;
        public IncrementalObservableGameViewModelCollection GamesIncrementalLoader
        {
            get { return _gamesIncrementalLoader; }
            set { SetProperty(ref _gamesIncrementalLoader, value); }
        }
        #endregion Games

        #endregion Properties

        #region Public Methods

        public async Task Load()
        {
            _gamesIncrementalLoader = new IncrementalObservableGameViewModelCollection(OnlineGoService);
        }

        #endregion Public Methods

        #region Commands
        
        #region NewGameCommand
        DelegateCommand _newGameCommand;
        public DelegateCommand NewGameCommand
        {
            get { if (_newGameCommand == null) _newGameCommand = new DelegateCommand(ExecuteNewGameCommand, CanNewGameCommand); return _newGameCommand; }
        }
        public bool CanNewGameCommand()
        {
            return true;
        }
        public void ExecuteNewGameCommand()
        {
             
        }
        #endregion NewGameCommand

        #endregion Commands

        #region Virtuals

        protected override void OnUserChanged()
        {
            base.OnUserChanged();

            if (IsConnected)
            {
                //_mail.Load();
            }
        }

        #endregion Virtuals

        #region Private Helpers and Event Handlers

        #endregion Private Helpers and Event Handlers
    }
}
