using System;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using GoG.WinRT.ViewModels;

namespace GoG.WinRT.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class GamePage
    {
        public GamePage()
        {
            this.InitializeComponent();

            // Set the min size to 250 * 400
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size { Width = 330, Height = 400 });

            this.SizeChanged += OnSizeChanged;
            this.DataContextChanged += OnDataContextChanged;
        }

        private GamePageViewModel _viewModel;

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            // Unsubscribe from old viewmodel's events.
            if (_viewModel != null)
                _viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            

            _viewModel = DataContext as GamePageViewModel;
            
            // Subscribe to new viewmodel's events.
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += ViewModelOnPropertyChanged;

                AdjustToVm(nameof(GamePageViewModel.WhoseTurn));
                AdjustToVm(nameof(GamePageViewModel.MessageText));
            }
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            AdjustToVm(propertyChangedEventArgs.PropertyName);
        }

        private void AdjustToVm(string propertyName)
        {
            if (_viewModel == null)
                return;

            switch (propertyName)
            {
                case nameof(GamePageViewModel.MessageText):
                    if (!String.IsNullOrEmpty(_viewModel.MessageText))
                    {
                        BigBoard.DisplayMessageAnimation();
                        SmallBoard.DisplayMessageAnimation();
                    }
                    else
                    {
                        BigBoard.HideMessageAnimation();
                        SmallBoard.HideMessageAnimation();
                    }
                    break;
                case nameof(GamePageViewModel.WhoseTurn):
                    if (_viewModel.WhoseTurn == 0)
                    {
                        BouncePlayer1Storyboard.Begin();
                        BouncePlayer2Storyboard.Stop();
                    }
                    else
                    {
                        BouncePlayer1Storyboard.Stop();
                        BouncePlayer2Storyboard.Begin();
                    }
                    break;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (sizeChangedEventArgs.NewSize.Width < sizeChangedEventArgs.NewSize.Height)
            {
                TopRow.Height = new GridLength(100, GridUnitType.Pixel);

                BigBoard.Visibility = Visibility.Collapsed;
                SmallBoard.Visibility = Visibility.Visible;
                LeftColumn.Width = new GridLength(1, GridUnitType.Star);
                RightColumn.Width = new GridLength(0, GridUnitType.Pixel);

                backButton.Style = Application.Current.Resources["SnappedBackButtonStyle"] as Style;
                pageTitle.Style = Application.Current.Resources["SnappedPageHeaderTextStyle"] as Style;

                Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                    {
                        var size = Math.Min(BottomRow.ActualHeight, LeftColumn.ActualWidth);
                        //SmallBoardScrollViewer.Width = SmallBoardScrollViewer.Height = size;
                        SmallBoard.Width = SmallBoard.Height = size;

                        SmallBoard.FixPieces();
                    });
            }
            else
            {
                TopRow.Height = new GridLength(140, GridUnitType.Pixel);

                BigBoard.Visibility = Visibility.Visible;
                SmallBoard.Visibility = Visibility.Collapsed;
                LeftColumn.Width = new GridLength(1, GridUnitType.Auto);
                RightColumn.Width = new GridLength(1, GridUnitType.Star);

                backButton.Style = Application.Current.Resources["BackButtonStyle"] as Style;
                pageTitle.Style = Application.Current.Resources["PageHeaderTextStyle"] as Style;

                Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                    {
                        var size = Math.Min(this.ActualHeight, RightColumn.ActualWidth);
                        //BigBoardScrollViewer.Width = BigBoardScrollViewer.Height = size;
                        BigBoard.Width = BigBoard.Height = size;

                        BigBoard.FixPieces();
                    });

            }
        }
    }
}
