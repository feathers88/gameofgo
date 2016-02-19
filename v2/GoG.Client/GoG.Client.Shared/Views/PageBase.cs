// ReSharper disable RedundantCatchClause
// ReSharper disable UnusedVariable
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantThisQualifier

using Windows.UI.Xaml.Controls;
using GoG.Client.ViewModels;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.StoreApps;

namespace GoG.Client.Views
{
    public class PageBase : VisualStateAwarePage
    {
        #region Data
        // CurrentPage used by popups to inject a Popup control and display a
        // user control in it.
        public static Page CurrentPage;
        #endregion Data

        #region Ctor and Init
        protected PageBase()
        {
            // This is where Prism locates our view model based on the name
            // of this class and some overrides in App.xaml.ca.
            SetValue(ViewModelLocator.AutoWireViewModelProperty, true);

            CurrentPage = this;
        }
        
        #endregion Ctor and Init

        #region Properties

        #region ViewModel
        public IPageViewModelBase ViewModel
        {
            get { return (IPageViewModelBase) DataContext; }
        }
        #endregion ViewModel

        #endregion Properties

        #region Private Helpers

        #endregion Private Helpers

        #region Virtuals

        #endregion Virtuals
    }
}
