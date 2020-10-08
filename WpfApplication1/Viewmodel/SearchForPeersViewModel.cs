using NetTools;
using P2P_Netwerken.ChatModels;
using P2P_Netwerken.View;
using P2P_Netwerken.Viewmodel.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;

namespace P2P_Netwerken.Viewmodel
{
    class SearchForPeersViewModel : INotifyPropertyChanged
    {

        private IpManager mgr;

        private string localIP;
        private Ping ping;
        private PingReply pingReply;

        private string ipAddress;
        private string hostName;


        private IPAddressRange range;
        public ICommand ScanForPeersButtonClickCommand { get; }
        public ICommand ConnectToPeerButtonClickCommand { get; }

        private double _CurrentProgress;

        private BackgroundWorker _worker;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<IPHostNameCombi> List { get; set; }

        private SynchronizationContext uiContext = SynchronizationContext.Current;

        public ObservableCollection<IPHostNameCombi> tl = new ObservableCollection<IPHostNameCombi>();

        private bool SearchDone;

        public SearchForPeersViewModel()
        {
            mgr = new IpManager();
            List = mgr.GetCombis();

            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerCompleted);

            ping = new Ping();
            ScanForPeersButtonClickCommand = new RelayCommand(execute => ScanForPeersButtonClick(), canExecute => CanExecuteScanForPeersButtonClick());
            ConnectToPeerButtonClickCommand = new RelayCommand(execute => ConnectToPeerButtonClick(), canExecute => CanExecuteConnectToPeerButtonClick());

        }

        private bool CanExecuteConnectToPeerButtonClick()
        {
           return true;
        }

        public bool Selected { get; set; }


        public double CurrentProgress
        {
            get { return _CurrentProgress; }
            private set
            {
                if (_CurrentProgress != value)
                {
                    _CurrentProgress = value;
                    OnPropertyChanged("CurrentProgress");

                }
            }
        }
        public string IpAddress
        {
            get { return ipAddress; }
            set
            {
                if (ipAddress != value)
                {
                    ipAddress = value;
                    OnPropertyChanged("IpAddress");
                }
            }
        }

        public string HostName
        {
            get { return hostName; }
            set
            {
                if (hostName != value)
                {
                    hostName = value;
                    OnPropertyChanged("HostName");
                }
            }
        }

        private IPHostNameCombi _selectedItem;

        public IPHostNameCombi SelectedItem
        {  
            get { return _selectedItem; }

            set
            {
                _selectedItem = value;
                Selected = true;
                OnPropertyChanged("SelectedItem");
            }
        
        }

        private void ConnectToPeerButtonClick()
        {
            if (Selected)
            {
                //initialize ChatProxy object to start a connection to the peer

                MessageBox.Show(SelectedItem.IpAddress);
            }
            else
            {
                MessageBox.Show("Please select a peer");

            }
        }
        private void ScanForPeersButtonClick()
        {
            SearchForPeers();

        }

        private void SearchForPeers()
        {
            if (!_worker.IsBusy && !SearchDone)
            {
                _worker.DoWork += Dowork_Ping;
                _worker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Searching for peers in progress or completed");
            }
        }

        private bool CanExecuteScanForPeersButtonClick()
        {
            return true;
        }


        private string GetLocalNetworkID()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                try
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint.Address.ToString();
                    string networkID = localIP.Substring(0, localIP.LastIndexOf(".") + 1);

                    return networkID;
                }
                catch
                {
                    MessageBox.Show("Uw lokale ipadres kan niet gevonden worden, probeer handmatig te verbinden.");
                }
            }
            return localIP;
        }
     
        private void Dowork_Ping(object sender, DoWorkEventArgs e)
        {
            var start = IPAddress.Parse(GetLocalNetworkID() + "0");
            var end = IPAddress.Parse(GetLocalNetworkID() + "20");

            var range = new IPAddressRange(start, end);

            double i = 0;
            foreach (IPAddress ip in range)
            {
                pingReply = ping.Send(GetLocalNetworkID() + i, 100);

                if (pingReply.Status == IPStatus.Success)
                {
                    uiContext.Send(x => List.Add(new IPHostNameCombi() { IpAddress = ip.ToString(), HostName = Dns.GetHostEntry(ip).HostName }), null);
                }

                Console.WriteLine(ip);

                _worker.ReportProgress((int)CalculateProgress(i, 19));
                i++;
            }
        }

        private double CalculateProgress(double count, double max)
        {
            double progress = (count / max) * 100;
            return progress;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _CurrentProgress = e.ProgressPercentage;
            OnPropertyChanged("CurrentProgress");
        }

        void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SearchDone = true;
            MessageBox.Show("Search complete, please click connect to begin chat");

        }

        // this method raises PropertyChanged event
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) // if there is any subscribers 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
