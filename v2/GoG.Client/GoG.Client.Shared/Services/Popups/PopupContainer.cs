using GoG.Client.Views;
// ReSharper disable CompareOfFloatsByEqualityOperator
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using GoG.Client.ViewModels;

namespace GoG.Client.Services.Popups
{
    public class PopupContainer
    {
        private Border _overlay;
        private readonly Grid _rootGrid;
        private Popup _popup;
        private Grid _interiorGrid;
        private InputPane _popupInputPane;
        private Rect _inputPaneRect;
        private Storyboard _storyboard;
        private PopupSettings _settings;
        private PopupWindowUserControl _window;
        private TaskCompletionSource<object> _closeTaskCompletionSource;
        private IPopupViewModelBase _popupViewModel;
        
        public PopupContainer(Grid rootGrid)
        {
            if (rootGrid == null) throw new ArgumentNullException("rootGrid");
            
            _rootGrid = rootGrid;
        }

        /// <summary>
        /// Opens a popup, returns when closes.  Inside user control's margin will be set to 20 by default.
        /// </summary>
        /// <param name="userControl">Any user control to display.</param>
        /// <param name="popupViewModel">A popup view model (with an IsOpen property).</param>
        /// <param name="settings">Default is centered.</param>
        public async Task ShowAsync(
            IPopupUserControlBase userControl, 
            IPopupViewModelBase popupViewModel, 
            PopupSettings settings = null)
        {
            if (userControl == null) throw new ArgumentNullException("userControl");
            
            if (_closeTaskCompletionSource != null)
                return;

            _popupViewModel = popupViewModel;

            if (settings == null)
                settings = PopupSettings.CenterWideDialog;
            _settings = settings;
        
            _popup = new Popup
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            // Will be placed inside rootGrid, which we don't control, so make sure
            // it fills the screen.
            _popup.SetValue(Grid.RowSpanProperty, int.MaxValue);
            _popup.SetValue(Grid.ColumnSpanProperty, int.MaxValue);

            Window.Current.SizeChanged += (sender, args) => AdjustSize();

            _interiorGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            _window = new PopupWindowUserControl(this)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            _window.InnerControl = userControl;
            
            if (_popupViewModel != null)
            {
                _window.DataContext = _popupViewModel;
                _popupViewModel.IsOpen = true;
                BindingOperations.SetBinding(_window, PopupWindowUserControl.TitleProperty,
                    new Binding
                    {
                        Path = new PropertyPath("Title"),
                    });
                BindingOperations.SetBinding(_window, PopupWindowUserControl.IsOpenAsyncProperty,
                    new Binding
                    {
                        Path = new PropertyPath("IsOpen"),
                        Mode = BindingMode.TwoWay,
                    });
            }

            // This may already have been set by the previous binding, but
            // we set it here again in case the view model .IsOpen was false.
            _window.IsOpenAsync = true;

            _overlay = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = new SolidColorBrush(Colors.Black),
                Opacity = _settings.OverlayOpacity
            };
            _overlay.SetValue(Grid.RowSpanProperty, int.MaxValue);
            _overlay.SetValue(Grid.ColumnSpanProperty, int.MaxValue);

            if (_settings.OverlayDismissal)
            {
                _overlay.Tapped += Overlay_Tapped;
                _overlay.IsTapEnabled = true;
            }

            _interiorGrid.Children.Add(_overlay);
            _interiorGrid.Children.Add(_window);

            _popup.Child = _interiorGrid;
            _popupInputPane = InputPane.GetForCurrentView();
            _popupInputPane.Showing += pane_Showing;
            _popupInputPane.Hiding += pane_Showing;

            _rootGrid.Children.Add(_popup);

            _popup.IsOpen = true;

            AdjustSize();

            await DoAnimationAsync(true);

            // When closed, _closeTaskCompletionSource will be completed.
            _popup.Closed += PopupOnClosed;
            _closeTaskCompletionSource = new TaskCompletionSource<object>();
            await _closeTaskCompletionSource.Task;
        }

        public void Cleanup()
        {
            // Clear source references so popup doesn't stick around.
            _window.ClearValue(PopupWindowUserControl.TitleProperty);
            _window.ClearValue(PopupWindowUserControl.IsOpenAsyncProperty);

            _popupViewModel = null;
            if (_closeTaskCompletionSource != null)
            {
                _closeTaskCompletionSource.SetCanceled();
                _closeTaskCompletionSource = null;
            }
            if (_overlay != null)
                _overlay.Tapped -= Overlay_Tapped;
            if (_window != null)
                _popup.Closed -= PopupOnClosed;
            if (_popup != null && _rootGrid != null && _rootGrid.Children != null)
            {
                _rootGrid.Children.Remove(_popup);
                _popup = null;
            }
            SetStoryboard(null);
        }

        /// <summary>
        /// Starts an animation and waits for it to complete.
        /// </summary>
        /// <param name="open"></param>
        /// <returns></returns>
        private Task DoAnimationAsync(bool open)
        {
            var board = CreateAnimation(open);
            SetStoryboard(board);
            return _storyboard.BeginAsync();
        }

        private void SetStoryboard(Storyboard board)
        {
            if (_storyboard != null)
            {
                _storyboard.Stop();
                _storyboard = null;
            }

            _storyboard = board;
        }

        private static TimeSpan TimeSpanMul(TimeSpan span, double mul)
        {
            return TimeSpan.FromMilliseconds(span.TotalMilliseconds * mul);
        }

        private Storyboard CreateBaseAnimation()
        {
            var board = new Storyboard();
            return board;
        }

        protected void AddFlipAnimation(Storyboard board, FrameworkElement element, bool open, TimeSpan duration)
        {
            var projection = element.Projection as PlaneProjection;
            if (projection == null)
                element.Projection = projection = new PlaneProjection();

            projection.CenterOfRotationY = 0.5;

            var flipAnim = new DoubleAnimationUsingKeyFrames();

            double start = open ? -90 : 0;
            double end = open ? 0 : 90;
            TimeSpan startTime;
            TimeSpan endTime;

            CalculateDurations(open, duration, ref startTime, ref endTime);

            Storyboard.SetTarget(flipAnim, projection);
            Storyboard.SetTargetProperty(flipAnim, "RotationX");

            flipAnim.KeyFrames.Add(new DiscreteDoubleKeyFrame() { KeyTime = TimeSpan.Zero, Value = start });
            flipAnim.KeyFrames.Add(new DiscreteDoubleKeyFrame() { KeyTime = startTime, Value = start });
            flipAnim.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = endTime, EasingFunction = GetEasingFunction(open), Value = end });

            board.Children.Add(flipAnim);
        }

        protected EasingFunctionBase GetEasingFunction(bool open)
        {
            return new QuadraticEase { EasingMode = open ? EasingMode.EaseOut : EasingMode.EaseIn };
        }

        private void CalculateDurations(bool open, TimeSpan duration, ref TimeSpan startTime, ref TimeSpan endTime)
        {
            startTime = _settings.AnimationDuration - duration;
            endTime = _settings.AnimationDuration;
            if (!open)
            {
                startTime = TimeSpan.Zero;
                endTime = duration;
            }
        }

        protected virtual Storyboard CreateAnimation(bool open)
        {
            var board = CreateBaseAnimation();
            if (_settings.Animation.IsFlagOn(PopupAnimation.ControlFlip))
            {
                AddFlipAnimation(board, _window, open, TimeSpanMul(_settings.AnimationDuration, _settings.OverlayToControlAnimationRatio));
            }
            else if (_settings.Animation.IsFlagOn(PopupAnimation.ControlFlyoutRight))
            {
                AddFlyoutAnimation(board, _window, open, TimeSpanMul(_settings.AnimationDuration, _settings.OverlayToControlAnimationRatio));
            }

            if (_settings.Animation.IsFlagOn(PopupAnimation.OverlayFade))
            {
                AddFadeAnimation(board, _overlay, open, _settings.OverlayOpacity, _settings.AnimationDuration);
            }

            return board;
        }

        private void AddFadeAnimation(Storyboard board, DependencyObject overlay, bool open, double fadeTo, TimeSpan duration)
        {
            var fadeAnim = new DoubleAnimationUsingKeyFrames();

            double start = open ? 0 : fadeTo;
            double end = open ? fadeTo : 0;

            Storyboard.SetTarget(fadeAnim, overlay);
            Storyboard.SetTargetProperty(fadeAnim, "Opacity");

            TimeSpan startTime;
            TimeSpan endTime;

            CalculateDurations(open, duration, ref startTime, ref endTime);

            fadeAnim.KeyFrames.Add(new DiscreteDoubleKeyFrame() { KeyTime = TimeSpan.Zero, Value = start });
            fadeAnim.KeyFrames.Add(new DiscreteDoubleKeyFrame() { KeyTime = startTime, Value = start });
            fadeAnim.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = endTime, Value = end, EasingFunction = GetEasingFunction(open), });

            board.Children.Add(fadeAnim);
        }

        private void AddFlyoutAnimation(Storyboard board, FrameworkElement element, bool open, TimeSpan duration)
        {
            var transform = element.RenderTransform as CompositeTransform;
            if (transform == null)
            {
                element.RenderTransform = transform = new CompositeTransform();
            }

            var flyoutAnim = new DoubleAnimationUsingKeyFrames();

            if (Double.IsNaN(element.Width) || (element.Width == 0))
                throw new InvalidOperationException("When animating as flyout, control width must be set");

            double start = open ? element.Width : 0;
            double end = open ? 0 : element.Width;
            TimeSpan startTime;
            TimeSpan endTime;

            CalculateDurations(open, duration, ref startTime, ref endTime);

            Storyboard.SetTarget(flyoutAnim, transform);
            Storyboard.SetTargetProperty(flyoutAnim, "TranslateX");

            flyoutAnim.KeyFrames.Add(new DiscreteDoubleKeyFrame() { KeyTime = TimeSpan.Zero, Value = start });
            flyoutAnim.KeyFrames.Add(new DiscreteDoubleKeyFrame() { KeyTime = startTime, Value = start });
            flyoutAnim.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = endTime, EasingFunction = GetEasingFunction(open), Value = end });

            board.Children.Add(flyoutAnim);
        }
        
        void pane_Showing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            _inputPaneRect = args.OccludedRect;
            AdjustSize();
        }

        async void Overlay_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _overlay.Tapped -= Overlay_Tapped;

            if (_popup.IsOpen)
                await CloseAsync();
        }

        public async Task CloseAsync()
        {
            if (_popup.IsOpen)
            {
                await DoAnimationAsync(false);
                _popup.IsOpen = false;
            }
        }

        private void AdjustSize()
        {
            var rect = Window.Current.CoreWindow.Bounds;
            if (_inputPaneRect.Top == 0)
                rect.Y = _inputPaneRect.Bottom;

            rect.Height -= _inputPaneRect.Height;

            _popup.HorizontalOffset = rect.Left;
            _popup.VerticalOffset = rect.Top;
            _interiorGrid.Width = rect.Width;
            _interiorGrid.Height = rect.Height;
        }

        private void PopupOnClosed(object sender, object o)
        {
            _closeTaskCompletionSource.SetResult(null);
            _closeTaskCompletionSource = null;

            Cleanup();
        }
    }
}
