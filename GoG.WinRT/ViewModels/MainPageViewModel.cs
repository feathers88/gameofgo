using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Unity;

namespace GoG.WinRT.ViewModels
{
    public class MainPageViewModel : PageViewModel
    {
        #region Ctor
        public MainPageViewModel(IUnityContainer c) : base(c)
        {

        }
        #endregion Ctor

        #region Commands

        #region SinglePlayerCommand
        DelegateCommand _singlePlayerCommand;
        public DelegateCommand SinglePlayerCommand
        {
            get { if (_singlePlayerCommand == null) _singlePlayerCommand = new DelegateCommand(ExecuteSinglePlayer, CanSinglePlayer); return _singlePlayerCommand; }
        }
        public bool CanSinglePlayer()
        {
            return true;
        }
        public void ExecuteSinglePlayer()
        {
            NavService.Navigate("SinglePlayer", null);
        }
        #endregion SinglePlayerCommand
      
        #endregion Commands
    }
}
