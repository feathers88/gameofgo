using Microsoft.Practices.Prism.StoreApps;

namespace GoG.WinRT80.ViewModels
{
    public class GameSettingsViewModel : ViewModel
    {
        private bool _isLastMoveIndicatorShowing;
        /// <summary>
        /// Indicate whether the previous move is indicated on the board.
        /// </summary>
        public bool IsLastMoveIndicatorShowing
        {
            get { return _isLastMoveIndicatorShowing; }
            set { SetProperty(ref _isLastMoveIndicatorShowing, value); }
        }

        private bool _areRecentCapturesShowing;
        /// <summary>
        /// Indicate the recent captures from the latest move.
        /// </summary>
        public bool AreRecentCapturesShowing
        {
            get { return _areRecentCapturesShowing; }
            set { SetProperty(ref _areRecentCapturesShowing, value); }
        }
    }
}
