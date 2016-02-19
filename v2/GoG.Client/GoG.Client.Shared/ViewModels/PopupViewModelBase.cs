using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.StoreApps;

namespace GoG.Client.ViewModels
{
    public interface IPopupViewModelBase : IViewModelBase
    {
        /// <summary>
        /// Gets or set whether the window is open or closed.
        /// </summary>
        bool IsOpen { get; set; }

        /// <summary>
        /// Bound to popup title.
        /// </summary>
        string Title { get; set; }
    }

    public class PopupViewModelBase : ViewModelBase, IPopupViewModelBase
    {
        #region Data

        #endregion Data

        #region Ctor and Init

        #endregion Ctor and Init

        #region Properties

        #region Title
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        #endregion Title

        #region IsOpen
        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set { SetProperty(ref _isOpen, value); }
        }
        #endregion IsOpen

        #endregion Properties

        #region Commands

        #region CloseCommand
        DelegateCommand _closeCommand;
        public DelegateCommand CloseCommand
        {
            get { if (_closeCommand == null) _closeCommand = new DelegateCommand(ExecuteCloseCommand, CanCloseCommand); return _closeCommand; }
        }
        public bool CanCloseCommand()
        {
            return true;
        }
        public void ExecuteCloseCommand()
        {
            IsOpen = false;
        }
        #endregion CloseCommand
        

        #endregion Commands
        
        #region IPopupViewModelBase Members

        #endregion
    }
}
