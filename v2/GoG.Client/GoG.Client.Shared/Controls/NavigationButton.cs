using System.Diagnostics;
using Windows.ApplicationModel.Store;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace GoG.Client.Controls
{
    public class NavigationButton : Button
    {
        #region Ctor

        public NavigationButton()
        {
            DefaultStyleKey = typeof(NavigationButton);
        }

        #endregion Ctor

        
        private void ApplyVisualState()
        {
            string state;

            if (IsSelected)
                state = "Selected";
            else if (!IsEnabled)
                state = "Disabled";
            else if (IsPressed)
                state = "Pressed";
            else if (IsPointerOver)
                state = "Hover";
            else
                state = "Normal";

            if (Dispatcher != null)
                Dispatcher.RunAsync(CoreDispatcherPriority.High,
                    () =>
                    {
                        var success = VisualStateManager.GoToState(this, state, true);
                        if (!success)
                            Debug.WriteLine("NavigationButton transition to state {0} failed.", state);
                    });
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            ApplyVisualState();
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            ApplyVisualState();
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            ApplyVisualState();
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            ApplyVisualState();
        }

        #region Dependency Properties

        #region IsSelected
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(NavigationButton), new PropertyMetadata(false, IsSelectedChangedCallback));
        private static void IsSelectedChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var navigationButton = dependencyObject as NavigationButton;
            if (navigationButton != null) 
                navigationButton.ApplyVisualState();
        }
        #endregion IsSelected

        #region Text
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(NavigationButton), new PropertyMetadata(null));
        #endregion Text

        #region Icon
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(NavigationButton), new PropertyMetadata(null));
        #endregion Icon

        #endregion Dependency Properties
    }
}
