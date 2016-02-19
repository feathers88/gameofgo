// ReSharper disable RedundantCatchClause
// ReSharper disable UnusedVariable
// ReSharper disable RedundantNameQualifier

using Microsoft.Practices.Prism.Mvvm;

namespace GoG.Client.ViewModels
{
    public interface IViewModelBase
    {
    }

    public abstract class ViewModelBase : BindableBase, IViewModelBase
    {
        #region Data

        #endregion Data

        #region Properties

        #region IsBusy
        private bool _isBusy;
        [RestorableState]
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }
        #endregion IsBusy

        #endregion Properties

        #region Virtuals

        

        #endregion Virtuals

        #region Private Helpers

        #endregion Private Helpers
    }
}
