using P2P_Netwerken.ChatBusiness;
using P2P_Netwerken.ChatModels;
using P2P_Netwerken.View;
using P2P_Netwerken.Viewmodel.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace P2P_Netwerken.Viewmodel
{
    class MainWindowViewModel
    {
        public ICommand SearchForPeersButtonClickCommand { get; }
        public ICommand TempChatButtonClickCommand { get; }


        public MainWindowViewModel()
        {
            SearchForPeersButtonClickCommand = new RelayCommand(execute => SearchForPeersButtonClick(execute), canExecute => CanExecuteSearchForPeersButtonClick());
            TempChatButtonClickCommand = new RelayCommand(execute => TempChatButtonClick(execute), canExecute => CanExecuteSearchForPeersButtonClick());

        }
        

        private void TempChatButtonClick(object parameter)
        {
            //open chat screen
            ChatView chatwindow = new ChatView
            {
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
            };

            ChatViewViewModel chatViewViewModel = new ChatViewViewModel();
            chatwindow.DataContext = chatViewViewModel;
            chatwindow.Show();

            (parameter as Window)?.Close();
        }

        private void SearchForPeersButtonClick(object parameter)
        {
            //open search for peers screen
            SearchForPeersView searchForPeersWindow = new SearchForPeersView
            {
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
            };

            SearchForPeersViewModel searchForPeersViewModel = new SearchForPeersViewModel();
            searchForPeersWindow.DataContext = searchForPeersViewModel;
            searchForPeersWindow.Show();

            (parameter as Window)?.Close();

        }

        private bool CanExecuteSearchForPeersButtonClick()
        {
            return true;
        }
    }
}
