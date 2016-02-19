using Windows.UI.Xaml.Controls;
using GoG.Client.ViewModels;

namespace GoG.Client.Views
{
    public abstract class PopupUserControlBase : UserControl, IPopupUserControlBase
    {
        protected T GetViewModel<T>() where T : class, IViewModelBase
        {
            return (T)DataContext;
        }

        #region IPopupUserControlBase Members

        public abstract Control GetInitiallyFocusedElement();

        #endregion
    }
}
