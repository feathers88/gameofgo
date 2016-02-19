using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GoG.Client.Models;

namespace GoG.Client.Controls
{
    public sealed class GamePiece : Button
    {
	    public GamePiece(string postion, double circleMargin, string sequence, GoColor? color, bool isHint, bool isNewPiece, bool isNewCapture)
        {
            DefaultStyleKey = typeof(GamePiece);

	        Position = postion;
            Sequence = sequence;
            Color = color;
            IsHint = isHint;
            IsLastMove = isNewPiece;
            IsNewCapture = isNewCapture;
            CircleMargin = new Thickness(circleMargin);
        }

        public string Position { get; set; }

        // These properties are from the PieceState.
        public GoColor? Color { get; set; }
        public bool IsHint { get; set; }
        public bool IsLastMove { get; set; }
        public bool IsNewCapture { get; set; }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (DesignMode.DesignModeEnabled)
                return;

            // Now we know that the template has been applied, we have a visual tree,
            // so state changes will work.
            UpdateVisualState(false);
        }
        
        internal void UpdateVisualState(bool useTransitions = true)
        {
            VisualStateManager.GoToState(this, IsHint ? "Hint" : "NoHint", useTransitions);
            VisualStateManager.GoToState(this, IsNewCapture ? "NewCapture" : "NoNewCapture", useTransitions);
            if (Color != null)
                VisualStateManager.GoToState(this, Color.ToString(), useTransitions);
            VisualStateManager.GoToState(this, Color == null ? "Blank" : "Taken", useTransitions);
            VisualStateManager.GoToState(this, (IsLastMove ? (Color + "LastMoveIndicator") : "NoLastMoveIndicator"), useTransitions);
        }
        
        #region Sequence
        public string Sequence
        {
            get { return (string)GetValue(SequenceProperty); }
            set { SetValue(SequenceProperty, value); }
        }
        public static readonly DependencyProperty SequenceProperty =
            DependencyProperty.Register("Sequence", typeof(string), typeof(GamePiece), new PropertyMetadata(null));
        #endregion Sequence

        #region CircleMargin
        public Thickness CircleMargin
        {
            get { return (Thickness)GetValue(CircleMarginProperty); }
            set { SetValue(CircleMarginProperty, value); }
        }
        public static readonly DependencyProperty CircleMarginProperty =
            DependencyProperty.Register("CircleMargin", typeof(Thickness), typeof(GamePiece), new PropertyMetadata(5D));
        #endregion CircleMargin
    }
}
