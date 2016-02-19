using System;
using GoG.Client.Services;
using Microsoft.Practices.Unity;

namespace GoG.Client.ViewModels
{
    public interface ILoginInstructionUserControlViewModel : IPopupViewModelBase
    {
        Uri ProfileUri { get; }
    }

    public class LoginInstructionUserControlViewModel : PopupViewModelBase, ILoginInstructionUserControlViewModel
    {
        #region Data

        #endregion Data

        #region Ctor and Init

        // Design mode ctor.
        public LoginInstructionUserControlViewModel()
        {
            
        }

        [InjectionConstructor]
        public LoginInstructionUserControlViewModel(
            IConfigurationService configurationService) 
        {
            Title = "Application Specific Password";

            ProfileUri = new Uri(configurationService.Settings.ProfileUri);
        }

        #endregion Ctor and Init

        #region ILoginInstructionUserControlViewModel Implementation
        #region ProfileUri
        private Uri _profileUri;
        public Uri ProfileUri
        {
            get { return _profileUri; }
            set { SetProperty(ref _profileUri, value); }
        }
        #endregion ProfileUri
        
        #endregion ILoginInstructionUserControlViewModel Implementation
    }
}
