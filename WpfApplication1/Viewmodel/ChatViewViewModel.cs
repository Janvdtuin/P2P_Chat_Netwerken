using Microsoft.Win32;
using P2P_Netwerken.ChatBusiness;
using P2P_Netwerken.ChatModels;
using P2P_Netwerken.Viewmodel.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace P2P_Netwerken.Viewmodel
{
    class ChatViewViewModel : INotifyPropertyChanged
    {
        #region ChatScreenBindings
        private string _ChatAreaText;
        public string ChatAreaText
        {
            get { return _ChatAreaText; }
            set
            {
                if (_ChatAreaText != value)
                {
                    _ChatAreaText = value;
                    NotifyPropertyChanged(nameof(ChatAreaText));
                }
            }
        }

        private string _Port;
        public string Port
        {
            get { return _Port; }
            set
            {
                if (_Port != value)
                {
                    _Port = value;
                    NotifyPropertyChanged(nameof(Port));
                }
            }
        }

        private string _Username;
        public string Username
        {
            get { return _Username; }
            set
            {
                if (_Username != value)
                {
                    _Username = value;
                    NotifyPropertyChanged(nameof(Username));
                }
            }
        }

        private string _UserInputText;
        public string UserInputText
        {
            get { return _UserInputText; }
            set
            {
                if (_UserInputText != value)
                {
                    _UserInputText = value;
                    NotifyPropertyChanged(nameof(UserInputText));
                }
            }
        }

        //Ipaddress retrieved from list of peers
        private string _IpAddress;
        public string IpAddress
        {
            get { return _IpAddress; }
            set
            {
                if (_IpAddress != value)
                {
                    _IpAddress = value;
                    NotifyPropertyChanged(nameof(IpAddress));
                }
            }
        }
        #endregion

        #region Commands
        public ICommand SendButtonClickCommand { get; }
        public ICommand StartChatButtonClickCommand { get; }
        public ICommand EnterDownCommand { get; }

        #endregion
        private ChatProxy _cp { get; set; }

        private string FilePath;
        public ChatViewViewModel()
        {

            SendButtonClickCommand = new RelayCommand(execute => SendButtonClick(), canExecute => true);

            StartChatButtonClickCommand = new RelayCommand(execute => StartChatButtonClick(), canExecute => true);

            EnterDownCommand = new RelayCommand(execute => OnEnterDown(), canExecute => true);

            Port = "900";
            IpAddress = "http://localhost:900";
            Username = "Jan";
        }

        private void OnEnterDown()
        {
            SendButtonClick();
        }

        private void SendButtonClick()
        {
            if (_cp != null)
            {
                if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(UserInputText))
                {
                    sendMessage(new Message(Username, UserInputText));
                }
                else
                {
                    ShowStatus("Chat not started");
                }
            }
        }

        private void StartChatButtonClick()
        {
            if (!string.IsNullOrEmpty(Port) && !string.IsNullOrEmpty(IpAddress))
            {
                _cp = new ChatProxy(ShowMessage, ShowStatus, Port, IpAddress);
                if (_cp.Status)
                {
                    ChatAreaText += ("Ready to chat");
                    ChatAreaText += Environment.NewLine;
                }
                }else
                {
                    ShowStatus("Please fill in all the fields");
                }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void ShowMessage(Message m)
        {
            ChatAreaText += ("[" + m.Sent + "] " + m.Username + ": " + m.Text);
            ChatAreaText += Environment.NewLine;
        }

        public void ShowStatus(string txt)
        {        
            MessageBox.Show(txt);                 
        }

        private void sendMessage(Message m)
        {
            _cp.SendMessage(m);
            UserInputText = "";
        }

        private void TestButtonClick()
        {
            UserInputText = "Test"; 
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
