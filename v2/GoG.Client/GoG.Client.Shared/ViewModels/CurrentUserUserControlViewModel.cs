namespace GoG.Client.ViewModels
{
    public class CurrentUserUserControlViewModel : ViewModelBase, ICurrentUserUserControlViewModel
    {
        #region UserId
        private string _userId;
        public string UserId
        {
            get { return _userId; }
            set { SetProperty(ref _userId, value); }
        }
        #endregion UserId

        #region Gravatar
        private string _gravatar;
        public string Gravatar
        {
            get { return _gravatar; }
            set { SetProperty(ref _gravatar, value); }
        }
        #endregion Gravatar
    }
}
