using System.Windows;
using P2P_Netwerken.Viewmodel;

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
