using GoG.Client.Models;
using GoG.Client.Services;
using GoG.Client.Services.Popups;
using GoG.Client.ViewModels;
using GoG.Client.ViewModels.Chat;
using GoG.Client.ViewModels.Pages;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Microsoft.Practices.Prism.Mvvm;

// ReSharper disable RedundantThisQualifier
// ReSharper disable ConvertToLambdaExpression

#if WINDOWS_PHONE_APP
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
#endif

namespace GoG.Client
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App
    {
        #region Data

#if WINDOWS_PHONE_APP
        private TransitionCollection _transitions;
#endif

        private readonly UnityContainer _container = new UnityContainer();
        //private TileUpdater _tileUpdater;

        public IEventAggregator EventAggregator { get; set; }

        #endregion Data

        /// <summary>
        /// Initializes the singleton instance of the <see cref="App"/> class. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            UnhandledException += OnUnhandledException;
            
            //this.Suspending += this.OnSuspending;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Debug.WriteLine("Unhandled exception: {0}", unhandledExceptionEventArgs.Exception);
            unhandledExceptionEventArgs.Handled = true;
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            NavigationService.Navigate<IMyGamesPageViewModel>();

            return Task.FromResult<object>(null);
        }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            // Register services.
            _container.RegisterInstance(NavigationService);
            _container.RegisterInstance(SessionStateService);
            _container.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());
            _container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));
            _container.RegisterType<IConfigurationService, ConfigurationService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IOAuth2IdentityService, OAuth2IdentityServiceProxy>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ICredentialStore, RoamingCredentialStore>(new ContainerControlledLifetimeManager());
            //this._container.RegisterType<ICacheService, TemporaryFolderCacheService>(new ContainerControlledLifetimeManager());
            //this._container.RegisterType<ISecondaryTileService, SecondaryTileService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IPopupService, PopupService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IGoGService, GoGService>(new ContainerControlledLifetimeManager());

            // register view models.
            _container.RegisterType<IChatViewModel, ChatViewModel>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ISignInPageViewModel, SignInPageViewModel>();
            _container.RegisterType<ILoginInstructionUserControlViewModel, LoginInstructionUserControlViewModel>(new ContainerControlledLifetimeManager());
            //this._container.RegisterType<IMailUserControlViewModel, MailUserControlViewModel>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IMyGamesPageViewModel, MyGamesPageViewModel>(new ContainerControlledLifetimeManager());

            return Task.FromResult<object>(null);
        }

        protected override object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        protected override void OnRegisterKnownTypesForSerialization()
        {
            base.OnRegisterKnownTypesForSerialization();
            SessionStateService.RegisterKnownType(typeof(UserInfo));
            SessionStateService.RegisterKnownType(typeof(UserDetails));
            SessionStateService.RegisterKnownType(typeof(UserSettings));
        }


        protected override Type GetPageType(string pageToken)
        {
            var ns = GetType().GetTypeInfo().Namespace;
            var pageNameWithParameter = String.Format(CultureInfo.InvariantCulture, "{0}.Views.Pages.{1}Page", ns, pageToken);
            var viewFullName = String.Format(CultureInfo.InvariantCulture, pageNameWithParameter, pageToken);
            var viewType = Type.GetType(viewFullName);
            
            return viewType; 
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            base.OnLaunched(e);

#if DEBUG
            if (Debugger.IsAttached)
            {
                //this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            
            ChangeViewModelResolution();

            // Documentation on working with tiles can be found at http://go.microsoft.com/fwlink/?LinkID=288821&clcid=0x409
            //_tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            //_tileUpdater.StartPeriodicUpdate(new Uri(Constants.ServerAddress + "/api/TileNotification"), PeriodicUpdateRecurrence.HalfHour);

        }

        private void ChangeViewModelResolution()
        {
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(
                viewType =>
                {
                    var viewModelTypeName = viewType.FullName.Replace(".Views.", ".ViewModels.") + "ViewModel";
                    var viewModelType = Type.GetType(viewModelTypeName);

                    return viewModelType;
                });
        }
        
#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            if (rootFrame != null)
            {
                rootFrame.ContentTransitions = this._transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
                rootFrame.Navigated -= this.RootFrame_FirstNavigated;
            }
        }
#endif

    }

    
}