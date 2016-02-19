using System;
using System.Collections.Generic;
using Windows.System;
using GoG.WinRT.Services;
using GoG.WinRT.ViewModels;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using Windows.ApplicationModel.Activation;
// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227
using NovaGS.WinRT;

namespace GoG.WinRT
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        // New up the singleton container that will be used for type resolution in the app
        readonly IUnityContainer _container = new UnityContainer();

        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Required override. Generally you do your initial navigation to launch page, or 
        /// to the page approriate based on a search, sharing, or secondary tile launch of the app
        /// </summary>
        /// <param name="args">The launch arguments passed to the application</param>
        protected override void OnLaunchApplication(LaunchActivatedEventArgs args)
        {
            // Use the logical name for the view to navigate to. The default convention
            // in the NavigationService will be to append "Page" to the name and look 
            // for that page in a .Views child namespace in the project. IF you want another convention
            // for mapping view names to view types, you can override 
            // the MvvmAppBase.GetPageNameToTypeResolver method
            NavigationService.Navigate("Main", null);
        }

        /// <summary>
        /// This is the place you initialize your services and set default factory or default resolver for the view model locator
        /// </summary>
        /// <param name="args">The same launch arguments passed when the app starts.</param>
        protected override void OnInitialize(IActivatedEventArgs args)
        {
            // Register MvvmAppBase services with the container so that view models can take dependencies on them
            _container.RegisterInstance<ISessionStateService>(SessionStateService);
            _container.RegisterInstance<INavigationService>(NavigationService);
            _container.RegisterInstance<IFlyoutService>(FlyoutService);
            // Register any app specific types with the container
            _container.RegisterType<IDataRepository, DataRepository>();
            _container.RegisterType<NovaGSHomePageViewModel>(new ContainerControlledLifetimeManager());
            _container.RegisterType<NovaGSClient>(new ContainerControlledLifetimeManager());
            _container.RegisterType<CustomSettingsFlyoutViewModel>(new ContainerControlledLifetimeManager());

            // Set a factory for the ViewModelLocator to use the container to construct view models so their 
            // dependencies get injected by the container.
            ViewModelLocator.SetDefaultViewModelFactory((viewModelType) => _container.Resolve(viewModelType));
        }

        protected override void OnRegisterKnownTypesForSerialization()
        {
            base.OnRegisterKnownTypesForSerialization();

            //SessionStateService.RegisterKnownType(typeof(Address));
        }

        protected override object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        protected override IList<SettingsCharmActionItem> GetSettingsCharmActionItems()
        {
            var settingsCharmItems = new List<SettingsCharmActionItem>
            {
                new SettingsCharmActionItem("How to Play", () => LoadWebPage("http://www.britgo.org/intro/intro.html")),
                new SettingsCharmActionItem("Suggestions and Bugs", () => LoadWebPage("mailto:gameofgo@outlook.com")),
                //new SettingsCharmActionItem("Project Site", () => LoadWebPage("https://gameofgo.codeplex.com/")),
                new SettingsCharmActionItem("Privacy", () => LoadWebPage("https://gameofgo.codeplex.com/wikipage?title=Privacy%20Statement")),
                new SettingsCharmActionItem("More Settings", () => FlyoutService.ShowFlyout("CustomSettings"))
            };

            return settingsCharmItems;
        }

        private void LoadWebPage(string url)
        {
            Launcher.LaunchUriAsync(new Uri(url));
        }

    }
}
