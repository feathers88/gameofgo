using Windows.UI.Xaml.Controls;

namespace GoG.Client.Views
{
    public interface IPopupUserControlBase : IUserControlBase
    {
        /// <summary>
        /// Gets the element you want to focus initially when the popup opens.
        /// This <u>must</u> return a control <b>not null</b> so that keyboard
        /// presses interact with the popup instead of the underlying frame
        /// (which could have unintended consequences).
        /// </summary>
        Control GetInitiallyFocusedElement();
    }
}
