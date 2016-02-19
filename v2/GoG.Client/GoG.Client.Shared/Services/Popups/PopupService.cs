using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using GoG.Client.ViewModels;
using GoG.Client.Views;
using Microsoft.Practices.Unity;

namespace GoG.Client.Services.Popups
{
    public class PopupService : IPopupService
    {
        #region Data
        private static bool _isShowing;
        private readonly IUnityContainer _container;
        private bool _popupIsOpen;
        #endregion Data

        #region Ctor and Init

        public PopupService(IUnityContainer container)
        {
            _container = container;
        }

        #endregion Ctor and Init

        #region IAlertMessageService Implementation

        public async Task<DialogCommand> ShowAsync(string message, string title)
        {
            return await ShowAsync(message, title, null);
        }

        public async Task<DialogCommand> ShowAsync(string message, string title, IEnumerable<DialogCommand> dialogCommands)
        {
            // Only show one dialog at a time.
            if (!_isShowing)
            {
                var messageDialog = new MessageDialog(message, title);

                DialogCommand[] dlgCmds = null;
                if (dialogCommands != null)
                    dlgCmds = dialogCommands as DialogCommand[] ?? dialogCommands.ToArray();
                if (dlgCmds != null)
                {
                    var commands = dlgCmds.Select(c => new UICommand(c.Label, command => { c.OnInvoked(); c.WasInvoked = true; }, c.Id));
                    foreach (var command in commands)
                        messageDialog.Commands.Add(command);
                }

                _isShowing = true;
                await messageDialog.ShowAsync();
                _isShowing = false;

                if (dlgCmds != null)
                    return dlgCmds.FirstOrDefault(dlgCmd => dlgCmd.WasInvoked);
                return null;
            }

            return null;
        }

        public async Task ShowAsync(IPopupViewModelBase viewModel)
        {
            if (_popupIsOpen)
                throw new InvalidOperationException("Can't call PopupService.ShowAsync() until the current popup is closed.");
            _popupIsOpen = true;
            try
            {
                var view = ResolveViewFromViewModel(viewModel.GetType());
                if (view is IPopupUserControlBase)
                {
                    var popupContainer = new PopupContainer(PageBase.CurrentPage.Content as Grid);
                    await popupContainer.ShowAsync(view as IPopupUserControlBase, viewModel);
                }
                else
                    throw new Exception(String.Format("Popup {0} does not implement IPopupUserControlBase.", view.GetType()));
            }
            finally
            {
                _popupIsOpen = false;
            }
        }

        #endregion IAlertMessageService Implementation

        #region Private Methods

        private IUserControlBase ResolveViewFromViewModel(Type viewModelType)
        {
            var typeFullName = viewModelType.FullName;

            if (!typeFullName.EndsWith("ViewModel"))
                throw new Exception("Popup service cannot resolve the view for \"" + typeFullName + "\" because it doesn't end in 'ViewModel'.");

            // Strip off "ViewModel".
            typeFullName = typeFullName.Substring(0, typeFullName.Length - 9);
            typeFullName = typeFullName.Replace(".ViewModels.", ".Views.");

            // If interface, remove any starting "I", which is the convention.
            if (typeFullName.StartsWith("I", StringComparison.OrdinalIgnoreCase) &&
                viewModelType.GetTypeInfo().IsInterface)
                typeFullName = typeFullName.Substring(1);

            var viewType = Type.GetType(typeFullName, true);
            var view = _container.Resolve(viewType);
            var rval = view as IUserControlBase;
            if (rval == null)
                throw new Exception("Popup service cannot resolve the view for \"" + typeFullName + "\" because the corresponding view isn't an IUserControlBase.");

            return rval;
        }

        #endregion Private Methods
    }
}
