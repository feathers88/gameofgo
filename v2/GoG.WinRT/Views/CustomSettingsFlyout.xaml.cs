using Windows.UI.Xaml.Navigation;
using GoG.WinRT.ViewModels;
using Microsoft.Practices.Prism.StoreApps;

namespace GoG.WinRT.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomSettingsFlyout : FlyoutView
    {
        public CustomSettingsFlyout()
            : base(StandardFlyoutSize.Narrow)
        {
            this.InitializeComponent();
        }
    }
}
