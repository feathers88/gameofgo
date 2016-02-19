using System.Windows.Input;
using Microsoft.Practices.Prism.StoreApps.Interfaces;

namespace GoG.Client.ViewModels
{
    public interface IPageViewModelBase : IViewModelBase, INavigationAware
    {
        ICommand GoBackCommand { get; }
        ICurrentUserUserControlViewModel CurrentUser { get; }
        string Title { get; }
    }
}
