using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using GoG.WinRT80.ViewModels;

namespace GoG.WinRT80.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SinglePlayerPage 
    {
        public SinglePlayerPage()
        {
            this.InitializeComponent();
            this.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (sizeChangedEventArgs.NewSize.Width < 535)
            {
                OptionsGrid.Margin = new Thickness(10,0,0,0);

                backButton.Style = Application.Current.Resources["SnappedBackButtonStyle"] as Style;
                pageTitle.Style = Application.Current.Resources["SnappedPageHeaderTextStyle"] as Style;
            }
            else
            {
                OptionsGrid.Margin = new Thickness(120, 0, 0, 0);

                backButton.Style = Application.Current.Resources["BackButtonStyle"] as Style;
                pageTitle.Style = Application.Current.Resources["PageHeaderTextStyle"] as Style;
            }
        }

        private void NameTB_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var vm = DataContext as SinglePlayerPageViewModel;
            if (vm != null)
                vm.Name = NameTB.Text;
        }

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
