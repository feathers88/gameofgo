using System;
using Windows.UI;
using Windows.UI.Xaml.Media;
using GoG.Client.ViewModels;

namespace GoG.Client.ViewModels.Chat
{
    public class ChatItemViewModel : ViewModelBase
    {
        #region Data



        #endregion Data

        #region Ctor and Init

        public ChatItemViewModel(string username, DateTime when, string message)
        {
            _username = username;
            _when = when;
            _whenAsShortString = _when.ToString("t");
            _message = message;
        }

        #endregion Ctor and Init

        #region Properties

        #region Username
        private readonly string _username;
        public string Username
        {
            get { return _username; }
        }
        #endregion Username

        #region When
        private readonly DateTime _when;
        public DateTime When
        {
            get { return _when; }
        }
        #endregion When

        #region WhenAsShortString
        private readonly string _whenAsShortString;
        public string WhenAsShortString
        {
            get { return _whenAsShortString; }
        }
        #endregion WhenAsShortString
        
        #region Message
        private readonly string _message;
        public string Message
        {
            get { return _message; }
        }
        #endregion Message

        #endregion Properties

        #region Commands



        #endregion Commands

        #region Virtuals



        #endregion Virtuals

        #region Private Helpers and Event Handlers



        #endregion Private Helpers and Event Handlers
    }
}
