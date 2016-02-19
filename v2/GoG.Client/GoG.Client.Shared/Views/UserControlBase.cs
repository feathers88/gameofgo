using Windows.UI.Xaml.Controls;
using GoG.Client.ViewModels;

namespace GoG.Client.Views
{
    public abstract class UserControlBase : UserControl, IUserControlBase
    {
        protected T GetViewModel<T>() where T : class, IViewModelBase
        {
            return (T)DataContext;
        }
    }
}
