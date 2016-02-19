// ReSharper disable ConvertToLambdaExpression

using System;
using Windows.UI.Xaml.Controls;
using GoG.Client.Views;
#pragma warning disable 4014
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace GoG.Client.Services.Popups
{
    public sealed partial class PopupWindowUserControl
    {
        #region Data

        private readonly PopupContainer _popupContainer;
        private Control _formerlyFocusedControl;

        #endregion Data

        #region Ctor and Init

        // For design time.
        public PopupWindowUserControl()
        {
            InitializeComponent();
        }

        // This is the runtime ctor.
        public PopupWindowUserControl(PopupContainer popupContainer)
        {
            InitializeComponent();
            _popupContainer = popupContainer;
        }

        #endregion Ctor and Init

        #region IsOpenAsync
        public bool IsOpenAsync
        {
            get { return (bool)GetValue(IsOpenAsyncProperty); }
            set { SetValue(IsOpenAsyncProperty, value); }
        }
        public static readonly DependencyProperty IsOpenAsyncProperty =
            DependencyProperty.Register("IsOpenAsync", typeof(bool), typeof(PopupWindowUserControl), new PropertyMetadata(false, IsOpenAsyncPropertyChangedCallback));
        
        private static void IsOpenAsyncPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var win = dependencyObject as PopupWindowUserControl;
            if (win == null) 
                return;
            win.TakeNewIsOpenAsync((bool)args.NewValue);
        }
        
        private async void TakeNewIsOpenAsync(bool p)
        {
            if (p)
            {
                // Opening, save formerly focused control so we can refocus later.
                var formerlyFocusedThing = FocusManager.GetFocusedElement();
                _formerlyFocusedControl = formerlyFocusedThing as Control;
                
                // Focus the initially focused control in our popup contents.
                if (InnerControl == null)
                    return;
                var focusControl = InnerControl.GetInitiallyFocusedElement();
                if (focusControl == null)
                    throw new Exception(String.Format("Popup user control {0}.GetInitiallyFocusElement() returned NULL, which is not allowed.", InnerControl.GetType().FullName));
                // Wait for window to fully open, then focus our control.
                Dispatcher.RunIdleAsync(
                    args =>
                    {
                        focusControl.Focus(FocusState.Programmatic);
                    });
                
            }
            else
            {
                await _popupContainer.CloseAsync();

                if (_formerlyFocusedControl != null)
                {
                    // Wait for window to close, then refocus original control.
                    Dispatcher.RunIdleAsync(
                    args =>
                    {
                        _formerlyFocusedControl.Focus(FocusState.Programmatic);
                    });
                }
            }
        }
        #endregion IsOpenAsync

        #region Title
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(PopupWindowUserControl), new PropertyMetadata(null));
        #endregion Title

        #region InnerControl
        public IPopupUserControlBase InnerControl
        {
            get { return (IPopupUserControlBase)GetValue(InnerControlProperty); }
            set { SetValue(InnerControlProperty, value); }
        }
        public static readonly DependencyProperty InnerControlProperty =
            DependencyProperty.Register("InnerControl", typeof(IUserControlBase), typeof(PopupWindowUserControl), new PropertyMetadata(null));
        
        #endregion InnerControl

        #region Events

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            _popupContainer.CloseAsync();
        }

        private void CloseBorder_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            CloseBorder.Background = new SolidColorBrush(Color.FromArgb(255, 227, 0, 0));
        }

        private void CloseBorder_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            CloseBorder.Background = new SolidColorBrush(Color.FromArgb(255, 137, 0, 0));
        }

        private void CloseBorder_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _popupContainer.CloseAsync();
        }

        #endregion Events
    }
}
