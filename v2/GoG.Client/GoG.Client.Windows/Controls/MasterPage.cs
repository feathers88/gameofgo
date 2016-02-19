using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GoG.Client.Controls
{
    public class MasterPage : Control
    {
        #region Ctor and Init

        public MasterPage()
        {
            DefaultStyleKey = typeof (MasterPage);
        }

        #endregion Ctor and Init

        #region Dependency Properties

        #region Body
        public object Body
        {
            get { return (object)GetValue(BodyProperty); }
            set { SetValue(BodyProperty, value); }
        }
        public static readonly DependencyProperty BodyProperty =
            DependencyProperty.Register("Body", typeof(object), typeof(MasterPage), new PropertyMetadata(null));
        #endregion Body

        #region NavigationVisibility
        public Visibility NavigationVisibility
        {
            get { return (Visibility)GetValue(NavigationVisibilityProperty); }
            set { SetValue(NavigationVisibilityProperty, value); }
        }
        public static readonly DependencyProperty NavigationVisibilityProperty =
            DependencyProperty.Register("NavigationVisibility", typeof(Visibility), typeof(MasterPage), new PropertyMetadata(Visibility.Visible));
        #endregion NavigationVisibility

        #region SignInVisibility
        public Visibility SignInVisibility
        {
            get { return (Visibility)GetValue(SignInVisibilityProperty); }
            set { SetValue(SignInVisibilityProperty, value); }
        }
        public static readonly DependencyProperty SignInVisibilityProperty =
            DependencyProperty.Register("SignInVisibility", typeof(Visibility), typeof(MasterPage), new PropertyMetadata(Visibility.Visible));
        #endregion SignInVisibility
        
        #endregion Dependency Properties
    }
}
