using P2P_Netwerken.ChatModels;
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
    class ChatViewViewModel
    {
        public ICommand TestButtonClickCommand { get; }


        public string ChatAreaText { get; set; }

        public string UserInputText { get; set; }

        public ChatViewViewModel()
        {
            TestButtonClickCommand = new RelayCommand(execute => TestButtonClick(), canExecute => true);

            //door middel van een singleton kunnen de items opgehaald worden uit het object. 
            //Dit moet ook gedaan worden bij de ChatProxy, deze moet een singleton worden waardoor er vanaf een andere klasse bij de properties kunnen komen.
            string test = TestObject.Instance.test;

            MessageBox.Show(test);




        }



        private void TestButtonClick()
        {
            MessageBox.Show("Click");
        }
    }
}
