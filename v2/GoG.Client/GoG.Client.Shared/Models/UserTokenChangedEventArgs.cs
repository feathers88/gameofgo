using System;

namespace GoG.Client.Models
{
    public class UserTokenChangedEventArgs : EventArgs
    {
        public UserTokenChangedEventArgs(string token)
        {
            _token = token;
        }

        #region Token
        private readonly string _token;
        public string Token
        {
            get { return _token; }
        }
        #endregion Token
    }
}
