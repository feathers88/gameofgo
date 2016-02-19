using System.Collections.ObjectModel;
using GoG.Client.ViewModels;
using Microsoft.Practices.Prism.StoreApps.Interfaces;

namespace GoG.Client.ViewModels
{
    public interface IDebugLogViewModel : INavigationAware
    {
    }

    public class DebugLogViewModel : ViewModelBase
    {
        #region Ctor and Init

        #endregion Ctor and Init

        #region Properties

        #region Items
        private ObservableCollection<string> _items = new ObservableCollection<string>();
        public ObservableCollection<string> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }
        #endregion Items

        #endregion Properties
    }
}
