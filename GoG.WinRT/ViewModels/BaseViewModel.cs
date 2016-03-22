using FuegoLib;
using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using GoG.Infrastructure;
using Microsoft.Practices.Prism.Mvvm;

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

        protected void RunOnUIThread(Action a)
        {
            if (Dispatcher != null)
                Dispatcher.RunAsync(CoreDispatcherPriority.Low,
                    () => a());
        }

        protected static async Task DisplayMessage(string title, string msg)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(msg, title);
            await dialog.ShowAsync();
        }

        protected static async Task DisplayErrorCode(GoResultCode code)
        {
            string msg;
            switch (code)
            {
                case GoResultCode.CommunicationError:
                    msg = "Communication error.  Are you connected to the Internet?";
                    break;
                case GoResultCode.EngineBusy:
                    msg = "Server is playing too many simultaneous games.  Please wait a minute and try again.";
                    break;
                case GoResultCode.OtherEngineError:
                case GoResultCode.ServerInternalError:
                    msg = "Something blew up!  Please try again.";
                    break;
                case GoResultCode.GameAlreadyExists:
                    msg = "Game already exists.  Please try again.";
                    break;
                case GoResultCode.GameDoesNotExist:
                    msg = "Your game was aborted due to inactivity.  Please start another one.";
                    break;
                case GoResultCode.ClientOutOfSync:
                    msg = "Your game was out of sync.";
                    break;
                case GoResultCode.SimultaneousRequests:
                    msg =
                        "Are you playing this game on another device right now?  If so, please leave and re-enter the game.";
                    break;
                case GoResultCode.Success:
                    msg = String.Empty;
                    break;
                case GoResultCode.IllegalMoveSpaceOccupied:
                    msg = "That space is occupied.";
                    break;
                case GoResultCode.IllegalMoveSuicide:
                    msg = "That move is suicide, which is not allowed.";
                    break;
                case GoResultCode.IllegalMoveSuperKo:
                    msg =
                        "That move would replicate a previous board position, which violates the \"superko\" rule.";
                    break;
                case GoResultCode.OtherIllegalMove:
                    msg = "That move is not legal.";
                    break;
                case GoResultCode.CannotScore:
                    msg =
                        "There are one or more stones that may be dead (or not).  Please continue playing until this situation is resolved.";
                    break;
                case GoResultCode.CannotSaveSGF:
                    msg = "Cannot save SGF.";
                    break;
                default:
                    throw new Exception("Unsupported value for GoResultCode: " + code);
            }
            await DisplayMessage("Whoops", msg);
        }

        #endregion Helpers
    }
}


