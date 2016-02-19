using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace GoG.Client.ViewModels.Chat
{
    public interface IChatViewModel : IViewModelBase
    {
        void AddChatItemAsync(string username, string message);
    }

    public class ChatViewModel : ViewModelBase, IChatViewModel
    {
        #region Data

        
        #endregion Data

        #region Ctor and Init

        public ChatViewModel()
        {
#if DEBUG
            if (DesignMode.DesignModeEnabled)
            {
                _message = "Hello, Nova.gs!";
                _items = new ObservableCollection<ChatItemViewModel>
                {
                    new ChatItemViewModel("christoban", DateTime.Now, "Hello, Nova.gs!"),
                    new ChatItemViewModel("misty mundae", DateTime.Now, "Mmm vampires..."),
                    new ChatItemViewModel("andyg", DateTime.Now, "@Misty What?"),
                    new ChatItemViewModel("christoban", DateTime.Now, "I'll take your vampire and raise you 7 billion zombies. 8-)"),
                    new ChatItemViewModel("misty mundae", DateTime.Now, "That would be the end of vampire kind - zombies taste bad.  I wonder what a vampire zombie would be like."),
                    new ChatItemViewModel("christoban", DateTime.Now, "Wouldn't last long in the sun.  But what does this have to do with Go?"),
                    new ChatItemViewModel("misty mundae", DateTime.Now, "I dunno.  I just like vampires! :-)"),
                };
            }
#endif
        }

        #endregion Ctor and Init

        #region Properties

        #region Items
        private ObservableCollection<ChatItemViewModel> _items = new ObservableCollection<ChatItemViewModel>();
        [RestorableState]
        public ObservableCollection<ChatItemViewModel> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }
        #endregion Items

        #region Message
        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                if (SetProperty(ref _message, value))
                    SendMessageCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion Message

        #endregion Properties

        #region Commands

        #region SendMessageCommand

        DelegateCommand _sendMessageCommand;
        public DelegateCommand SendMessageCommand
        {
            get { if (_sendMessageCommand == null) _sendMessageCommand = new DelegateCommand(ExecuteSendMessageCommand, CanSendMessageCommand); return _sendMessageCommand; }
        }
        public bool CanSendMessageCommand()
        {
            return !String.IsNullOrWhiteSpace(_message);
        }
        public void ExecuteSendMessageCommand()
        {
            AddChatItemAsync("christoban", _message); 
        }
        #endregion SendMessageCommand<string>
      
        #endregion Commands

        #region Virtuals



        #endregion Virtuals

        #region Private Helpers and Event Handlers



        #endregion Private Helpers and Event Handlers

        #region IChatViewModel Members

        public void AddChatItemAsync(string username, string message)
        {
            Items.Add(new ChatItemViewModel(username, DateTime.Now, message));
        }

        #endregion
    }
}
