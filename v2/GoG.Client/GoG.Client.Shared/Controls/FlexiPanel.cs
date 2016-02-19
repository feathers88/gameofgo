using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GoG.Client.Controls
{
    public class FlexiPanel : Panel
    {
        #region Data

        private double _totChildSize;

        #endregion Data

        #region Dependency Properties

        #region Orientation

        public Orientation Orientation
        {
            get { return (Orientation) GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof (Orientation), typeof (FlexiPanel),
                new PropertyMetadata(Orientation.Horizontal, OrientationPropertyChangedCallback));

        private static void OrientationPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var obj = dependencyObject as FlexiPanel;
            if (obj != null)
                obj.InvalidateArrange();
        }

        #endregion Orientation

        public static GridLength GetSize(DependencyObject obj)
        {
            return (GridLength) obj.GetValue(SizeProperty);
        }

        public static void SetSize(DependencyObject obj, GridLength value)
        {
            obj.SetValue(SizeProperty, value);
        }

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.RegisterAttached("Size", typeof (GridLength), typeof (FlexiPanel),
                new PropertyMetadata(GridLength.Auto));

        #endregion Dependency Properties

        #region Overrides

        protected override Size MeasureOverride(Size availableSize)
        {
            // For the measure step, we ask for the desired size of the children, given infinite space.
            // In the arrange step, if that total size is larger than the total available, we limit the 
            // star-sized children to the remainder.  Otherwise, we put the empty space at the end.

            _totChildSize = 0.0;
            foreach (var child in Children)
            {
                child.Measure(new Size(double.PositiveInfinity, availableSize.Height));
                _totChildSize += child.DesiredSize.Width;
            }
            return new Size(_totChildSize, availableSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var curPosition = 0.0;

            if (_totChildSize <= finalSize.Width)
            {
                // All fit within final size, arrange like a StackPanel.
                foreach (var child in Children)
                {
                    child.Arrange(new Rect(curPosition, 0, child.DesiredSize.Width, finalSize.Height));
                    curPosition += child.DesiredSize.Width;
                }
            }
            else
            {
                // All children don't fit in final size, so we arrange like a Grid.

                // First, get remaining space available to star-sized children, and the total star
                // multiplier so we can later determine the amount to apportion each star-size child.
                var remainingSpace = finalSize.Width;
                var totalStarMultipliers = 0.0;
                foreach (var child in Children)
                {
                    var childSize = GetSize(child);

                    if (childSize.IsStar)
                        totalStarMultipliers += childSize.Value;
                    else
                        remainingSpace -= child.DesiredSize.Width;
                }
                remainingSpace = remainingSpace < 0 ? 0 : remainingSpace;

                // Now, we have enough information to apportion space to each child.
                foreach (var child in Children)
                {
                    var childSize = GetSize(child);

                    double givenSize = childSize.IsStar
                        ? remainingSpace*(childSize.Value/totalStarMultipliers)
                        : child.DesiredSize.Width;

                    child.Arrange(new Rect(curPosition, 0, givenSize, finalSize.Height));
                    
                    curPosition += givenSize;
                }
            }

            // We use the entire size.
            return finalSize;
        }

        #endregion Overrides
    }
}
