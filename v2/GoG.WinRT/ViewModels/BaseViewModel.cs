using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using GoG.Infrastructure;
using GoG.Infrastructure.Engine;
using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;

namespace GoG.WinRT.ViewModels
{
    public class BaseViewModel : ViewModel
    {
        #region Helpers

        protected static readonly CoreDispatcher Dispatcher;

        static BaseViewModel()
        {
            Dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        }

        protected async Task RunOnUIThread(Action a)
        {
            if (Dispatcher != null)
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low,
                                          () => a());
        }

        protected async Task DisplayMessage(string title, string msg)
        {
            await RunOnUIThread(async () =>
            {
                var dialog = new Windows.UI.Popups.MessageDialog(msg, title);
                try
                {
                    await dialog.ShowAsync();
                }
                catch
                {
                }
                
            });

        }

        protected async Task DisplayErrorCode(GoResultCode code)
        {
            var msg = EngineHelpers.GetResultCodeFriendlyMessage(code);
            await DisplayMessage("Whoops", msg);
        }

        #endregion Helpers
    }
}
