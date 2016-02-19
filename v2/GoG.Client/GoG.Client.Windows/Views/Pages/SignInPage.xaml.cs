using GoG.Client.ViewModels.Pages;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace GoG.Client.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignInPage
    {
        public SignInPage()
        {
            InitializeComponent();

            IsEnabledChanged += SignInUserControl_IsEnabledChanged;
            PasswordBox.KeyDown += PasswordBox_KeyDown;

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var vm = (SignInPageViewModel) DataContext;
            if (vm != null)
                vm.InvalidPasswordEvent += VmOnInvalidPasswordEvent;
        }

        private Storyboard sb;

        private void VmOnInvalidPasswordEvent(object sender, EventArgs eventArgs)
        {
            if (sb == null)
            {
                sb = PasswordBox.Resources["ShakePasswordStoryboard"] as Storyboard;
                if (sb != null)
                    sb.Completed += (o, a) => sb.Stop();
            }
            if (sb != null)
            {
                Storyboard.SetTarget(sb, PasswordBox);
                if (sb.GetCurrentState() == ClockState.Stopped)
                    sb.Begin();
            }
            PasswordBox.Focus(FocusState.Programmatic);
        }
        
        void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && SubmitButton != null && SubmitButton.Command != null)
            {
                e.Handled = true;
                SubmitButton.Command.Execute(null);
            }
        }

        void SignInUserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible && Username != null) 
                Username.Focus(FocusState.Programmatic);
        }
    }
}
