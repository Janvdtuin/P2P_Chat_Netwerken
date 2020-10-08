using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Web.Http.SelfHost;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Threading.Tasks;
using P2P_Netwerken.Viewmodel;
using P2P_Netwerken.View;

namespace P2P_Netwerken
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            P2P_Netwerken.View.MainWindowView window = new P2P_Netwerken.View.MainWindowView
            {
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
            };
            MainWindowViewModel viewModel = new MainWindowViewModel();
            window.DataContext = viewModel;

            window.Show();
        }
    }
}
