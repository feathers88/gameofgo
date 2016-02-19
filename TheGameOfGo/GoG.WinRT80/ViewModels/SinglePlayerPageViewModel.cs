using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Windows.UI.Xaml.Navigation;
using GoG.Infrastructure.Engine;
using GoG.Infrastructure.Services.Engine;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Unity;

namespace GoG.WinRT80.ViewModels
{
    public class SinglePlayerPageViewModel : PageViewModel
    {
        #region Ctor
        public SinglePlayerPageViewModel(IUnityContainer c) : base(c)
        {
            _boardEdgeSize = 9;
            _sizes = new List<Pair>
                {
                    new Pair("9x9 (Small)", 9),
                    new Pair("13x13 (Medium)", 13),
                    new Pair("19x19 (Full Size)", 19)
                };
            //_secondsPerTurn = 15;
            //_seconds = new List<Pair>
            //    {
            //        new Pair("No Limit", 0),
            //        new Pair("10 Seconds", 10),
            //        new Pair("15 Seconds", 15),
            //        new Pair("20 Seconds", 20),
            //        new Pair("30 Seconds", 30),
            //    };
            //for (int i = 3; i <= 180; i++)
            //    _seconds.Add(new Pair(i + " Seconds", i));
            _difficultyLevel = 6;
            _difficulties = new List<Pair>
                {
                    new Pair("1 (Novice)", 0),
                    new Pair("2", 1),
                    new Pair("3", 2),
                    new Pair("4 (Easy)", 3),
                    new Pair("5", 4),
                    new Pair("6", 5),
                    new Pair("7 (Normal)", 6),
                    new Pair("8", 7),
                    new Pair("9", 8),
                    new Pair("10 (Hard)", 9),
                };
            _color = (int)GoColor.Black;
            _colors = new List<Pair>
                {
                    new Pair("Black", (int)GoColor.Black),
                    new Pair("White", (int)GoColor.White)
                };
            _name = "Homer";
            //_komi = (decimal)4.5;
            
        }
        #endregion Ctor

        #region Properties

        #region ActiveGame
        private Guid _activeGame;
        [RestorableState]
        // The server side guid storing our full game state.  The value is used to
        // load the value of the other properties from the server.
        public Guid ActiveGame
        {
            get { return _activeGame; }
            set
            {
                SetProperty(ref _activeGame, value);
                OnPropertyChanged("IsActiveGame");
            }
        }
        #endregion ActiveGame

        #region IsActiveGame
        public bool IsActiveGame
        {
            get { return _activeGame != Guid.Empty; }
        }
        #endregion IsActiveGame

        //#region ActiveGameState
        //private GoGameState _activeGameState;
        //public GoGameState ActiveGameState
        //{
        //    get { return _activeGameState; }
        //    set { _activeGameState = value; OnPropertyChanged("ActiveGameState"); }
        //}
        //#endregion ActiveGameState

        private string _name;
        [RestorableState]
        [Required(ErrorMessage = "Name is required.")]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); PlayCommand.RaiseCanExecuteChanged(); }
        }

        private List<Pair> _sizes;
        public List<Pair> Sizes
        {
            get { return _sizes; }
            set { SetProperty(ref _sizes, value); }
        }

        //#region Seconds
        //private List<Pair> _seconds;
        //public List<Pair> Seconds
        //{
        //    get { return _seconds; }
        //    set { _seconds = value; OnPropertyChanged("Seconds"); }
        //}
        //#endregion Seconds

        private List<Pair> _difficulties;
        public List<Pair> Difficulties
        {
            get { return _difficulties; }
            set { SetProperty(ref _difficulties, value); }
        }

        private int _boardEdgeSize;
        [RestorableState]
        public int BoardEdgeSize
        {
            get { return _boardEdgeSize; }
            set { SetProperty(ref _boardEdgeSize, value); }
        }

        private int _difficultyLevel;
        [RestorableState]
        public int DifficultyLevel
        {
            get { return _difficultyLevel; }
            set { SetProperty(ref _difficultyLevel, value); }
        }

        private List<Pair> _colors;
        public List<Pair> Colors
        {
            get { return _colors; }
            set { SetProperty(ref _colors, value); }
        }

        private int _color;
        [RestorableState]
        public int Color
        {
            get { return _color; }
            set { SetProperty(ref _color, value); }
        }

        //private decimal _komi;
        //[RestorableState]
        //public decimal Komi
        //{
        //    get { return _komi; }
        //    set { SetProperty(ref _komi, value); }
        //}

        //#region SecondsPerTurn
        //private int _secondsPerTurn;
        //[RestorableState]
        //public int SecondsPerTurn
        //{
        //    get { return _secondsPerTurn; }
        //    set { _secondsPerTurn = value; OnPropertyChanged("SecondsPerTurn"); }
        //}
        //#endregion SecondsPerTurn

        #endregion Properties

        #region Commands

        #region PlayCommand
        DelegateCommand _playCommand;
        public DelegateCommand PlayCommand
        {
            get { return _playCommand ?? (_playCommand = new DelegateCommand(ExecutePlay, CanPlay)); }
        }
        public bool CanPlay()
        {
            return !String.IsNullOrWhiteSpace(Name);
        }
        public void ExecutePlay()
        {
            // Start a new game by calling server, then go to game page.
            StartNewGame();
        }
        #endregion PlayCommand

        
        #region ResumeCommand
        DelegateCommand _resumeCommand;
        public DelegateCommand ResumeCommand
        {
            get { return _resumeCommand ?? (_resumeCommand = new DelegateCommand(ExecuteResume, CanResume)); }
        }
        public bool CanResume()
        {
            return true;
        }
        public void ExecuteResume()
        {
            NavService.Navigate("Game", ActiveGame);
        }
        #endregion ResumeCommand
      

        #endregion Commands

        #region Virtuals
        
        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewState)
        {
            // This parent call restores the [RestorableState] properties.
            base.OnNavigatedTo(navigationParameter, navigationMode, viewState);

            this.RunOnUIThread(async () =>
            {
                var msg = await DataRepository.GetActiveMessage();
                if (!String.IsNullOrWhiteSpace(msg))
                    DisplayMessage("The Game of Go", msg);
            });

            // Just a quick check to see if ActiveGame still exists.  If the user was away for very long,
            // it could easily have been deleted by the server.
            if (ActiveGame != Guid.Empty)
                CheckActiveGameExists();
        }

        /// <summary>
        /// Checks the active game exists on the server.  If not, the active game is erased.  If
        /// there is any kind of error, we ignore it and assume the game exists (we'll be more
        /// thorough when we load the game viewmodel).
        /// </summary>
        private async void CheckActiveGameExists()
        {
            BusyMessage = "Syncronizing...";
            IsBusy = true;
            var resp = await DataRepository.GetGameExists(ActiveGame);
            IsBusy = false;

            if (resp.ResultCode == GoResultCode.GameDoesNotExist)
                ActiveGame = Guid.Empty;
        }

        #endregion Virtuals

        #region Helpers

        private async void StartNewGame()
        {
            try
            {
                bool success = false;
                GoGameStateResponse resp = null;

                for (int tries = 0; !AbortOperation && !success && tries < 5; tries++)
                {
                    BusyMessage = "Starting game...";
                    IsBusy = true;

                    var tmpNewGame = Guid.NewGuid();

                    // Create game from user's selections.
                    var p1 = new GoPlayer();
                    var p2 = new GoPlayer();
                    if (Color == (int) GoColor.Black)
                    {
                        p1.Name = Name;
                        p1.PlayerType = PlayerType.Human;

                        p2.Name = "Fuego";
                        p2.PlayerType = PlayerType.AI;
                        p2.Level = DifficultyLevel;
                    }
                    else
                    {
                        p2.Name = Name;
                        p2.PlayerType = PlayerType.Human;

                        p1.Name = "Fuego";
                        p1.PlayerType = PlayerType.AI;
                        p1.Level = DifficultyLevel;

                    }
                    var tmpState = new GoGameState((byte) BoardEdgeSize,
                        p1, p2,
                        GoGameStatus.Active,
                        GoColor.Black,
                        "",
                        "",
                        null,
                        0);
                    resp = await DataRepository.StartAsync(tmpNewGame, tmpState);
                    BusyMessage = null;
                    IsBusy = false;

                    if (resp.ResultCode == GoResultCode.Success)
                    {
                        ActiveGame = tmpNewGame;
                        success = true;
                    }
                }

                if (AbortOperation)
                    return;

                if (success)
                {
                    NavService.Navigate("Game", ActiveGame);
                }
                else
                {
                    if (resp != null)
                        await DisplayErrorCode(resp.ResultCode);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                BusyMessage = null;
                IsBusy = false;
            }
        }

        #endregion Helpers
    }

    public class Pair
    {
        public Pair(string desc, int value)
        {
            Desc = desc;
            Value = value;
        }

        public string Desc { get; set; }
        public int Value { get; set; }
    }
}
