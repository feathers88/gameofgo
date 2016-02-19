using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using GoG.WinRT.ViewModels;
using Windows.UI.Xaml.Controls;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace GoG.WinRT.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class NovaGSHomePage 
    {
        public NovaGSHomePage()
        {
            this.InitializeComponent();
            this.SizeChanged += OnSizeChanged;
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
                //pageTitle.Style = Application.Current.Resources["SnappedPageHeaderTextStyle"] as Style;

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
                //pageTitle.Style = Application.Current.Resources["PageHeaderTextStyle"] as Style;

                Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    var size = Math.Min(this.ActualHeight, RightColumn.ActualWidth);
                    //BigBoardScrollViewer.Width = BigBoardScrollViewer.Height = size;
                    BigBoard.Width = BigBoard.Height = size;

                    BigBoard.FixPieces();
                });

            }
        }

        //private void NameTB_OnTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    var vm = DataContext as SinglePlayerPageViewModel;
        //    if (vm != null)
        //        vm.Name = NameTB.Text;
        //}

        //private void KomiTB_OnTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (KomiTB.Text.Length == 0) return;

        //    var text = KomiTB.Text;

        //    decimal result;
        //    var isValid = decimal.TryParse(text, out result);
        //    if (isValid) return;

        //    KomiTB.Text = text.Remove(text.Length - 1);
        //    KomiTB.SelectionStart = text.Length;

        //    var vm = DataContext as SinglePlayerPageViewModel;
        //    if (vm != null)
        //        vm.Komi = result;
        //}
    }
}
