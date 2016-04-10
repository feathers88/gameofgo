using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GoG.Infrastructure.Engine;
using GoG.WinRT.Services;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.StoreApps;

namespace GoG.WinRT
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : MvvmAppBase
    {
        private bool _isRestoringFromTermination;

        public App()
        {
            this.InitializeComponent();
            this.RequestedTheme = ApplicationTheme.Dark;
        }

        // New up the singleton container that will be used for type resolution in the app
        readonly IUnityContainer _container = new UnityContainer();
        
        protected override void OnRegisterKnownTypesForSerialization()
        {
            base.OnRegisterKnownTypesForSerialization();

            SessionStateService.RegisterKnownType(typeof(GoGameState));
            SessionStateService.RegisterKnownType(typeof(GoPlayer));
            SessionStateService.RegisterKnownType(typeof(PlayerType));
            SessionStateService.RegisterKnownType(typeof(GoGameStatus));
            SessionStateService.RegisterKnownType(typeof(GoColor));
            SessionStateService.RegisterKnownType(typeof(MoveType));
            SessionStateService.RegisterKnownType(typeof(GoOperation));
            SessionStateService.RegisterKnownType(typeof(MoveType));
            SessionStateService.RegisterKnownType(typeof(GoResultCode));
            SessionStateService.RegisterKnownType(typeof(GoMoveHistoryItem));
            SessionStateService.RegisterKnownType(typeof(GoMove));
            SessionStateService.RegisterKnownType(typeof(GoMoveResult));
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PrelaunchActivated)
            {
                return;
            }

            base.OnLaunched(args);
        }

        /// <summary>
        /// Required override. Generally you do your initial navigation to launch page, or 
        /// to the page approriate based on a search, sharing, or secondary tile launch of the app
        /// </summary>
        /// <param name="args">The launch arguments passed to the application</param>
        protected override async Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            // Use the logical name for the view to navigate to. The default convention
            // in the NavigationService will be to append "Page" to the name and look 
            // for that page in a .Views child namespace in the project. IF you want another convention
            // for mapping view names to view types, you can override 
            // the MvvmAppBase.GetPageNameToTypeResolver method
            if (args.PreviousExecutionState != ApplicationExecutionState.Terminated)
                NavigationService.Navigate("SinglePlayer", null);
        }

        

        /// <summary>
        /// This is the place you initialize your services and set default factory or default resolver for the view model locator
        /// </summary>
        /// <param name="args">The same launch arguments passed when the app starts.</param>
        protected override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            // Register MvvmAppBase services with the container so that view models can take dependencies on them
            _container.RegisterInstance<ISessionStateService>(SessionStateService);
            _container.RegisterInstance<INavigationService>(NavigationService);
            // Register any app specific types with the container
            _container.RegisterType(typeof(IDataRepository), typeof(DataRepository), new ContainerControlledLifetimeManager());

            // Set a factory for the ViewModelLocator to use the container to construct view models so their 
            // dependencies get injected by the container
            ViewModelLocationProvider.SetDefaultViewModelFactory((viewModelType) => _container.Resolve(viewModelType));

        }
        
        protected override object Resolve(Type type)
        {
            return _container.Resolve(type);
        }
    }
}
