using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GoG.Client.ViewModels;

namespace GoG.Client.Views
{
    public sealed partial class LoginInstructionUserControl : PopupUserControlBase
    {
        public LoginInstructionUserControl()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var vm = GetViewModel<LoginInstructionUserControlViewModel>();
            if (vm != null)
                ProfileLink.NavigateUri = vm.ProfileUri;
        }

        public override Control GetInitiallyFocusedElement()
        {
            return OkButton;
        }
    }
}
