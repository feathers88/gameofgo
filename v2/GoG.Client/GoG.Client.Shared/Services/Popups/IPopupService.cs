using System.Collections.Generic;
using System.Threading.Tasks;
using GoG.Client.ViewModels;

namespace GoG.Client.Services.Popups
{
    public interface IPopupService
    {
        Task<DialogCommand> ShowAsync(string message, string title);
        Task<DialogCommand> ShowAsync(string message, string title, IEnumerable<DialogCommand> dialogCommands);
        
        /// <summary>
        /// Displays a popup containing the UI corresponding to the viewModel.  Returns when the popup closes.
        /// </summary>
        /// <param name="viewModel"></param>
        Task ShowAsync(IPopupViewModelBase viewModel);
    }
}
