using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using GoG.WinRT.ViewModels;
using Windows.UI.Xaml.Controls;

namespace GoG.WinRT.Views
{
    public sealed partial class SinglePlayerPage 
    {
        public SinglePlayerPage()
        {
            this.InitializeComponent();

            // Set the min size to 250 * 400
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size { Width = 330, Height = 400 });

            this.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (sizeChangedEventArgs.NewSize.Width < 535)
            {
                OptionsGrid.Margin = new Thickness(10,0,0,0);

                BackButton.Style = Application.Current.Resources["SnappedBackButtonStyle"] as Style;
                PageTitle.Style = Application.Current.Resources["SnappedPageHeaderTextStyle"] as Style;
            }
            else
            {
                OptionsGrid.Margin = new Thickness(120, 0, 0, 0);

                BackButton.Style = Application.Current.Resources["BackButtonStyle"] as Style;
                PageTitle.Style = Application.Current.Resources["PageHeaderTextStyle"] as Style;
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
