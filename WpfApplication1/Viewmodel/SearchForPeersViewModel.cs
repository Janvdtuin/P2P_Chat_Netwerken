﻿using NetTools;
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

        private string localIP;
        private Ping ping;
        private string ipAddress;
        private string hostName;
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
            //List which contains a combination of IP address and hostname
            List = new ObservableCollection<IPHostNameCombi>();

            //create new backgroundworker to search for peers without freezing the GUI
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerCompleted);

            ping = new Ping();
            ScanForPeersButtonClickCommand = new RelayCommand(execute => ScanForPeersButtonClick(), canExecute => CanExecuteScanForPeersButtonClick());
            ConnectToPeerButtonClickCommand = new RelayCommand(execute => ConnectToPeerButtonClick(execute), canExecute => CanExecuteConnectToPeerButtonClick());

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

        private void ConnectToPeerButtonClick(object parameter)
        {
            if (Selected)
            {
                //open ChatView window with the credentials from peerlist filled in.
                ChatView chatwindow = new ChatView
                {
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
                };

                ChatViewViewModel chatViewViewModel = new ChatViewViewModel();
                chatViewViewModel.IpAddress = "http://" + SelectedItem.IpAddress + ":900";
                chatwindow.DataContext = chatViewViewModel;
                chatwindow.Show();

                (parameter as Window)?.Close();
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

        //returns the local network id by connecting to a socket and reading its endpoint.
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
            string ipBase = GetLocalNetworkID();

            for (int i = 0; i < 255; i++)
            {
                string ip = ipBase + i.ToString();

                Ping p = new Ping();
                p.PingCompleted += new PingCompletedEventHandler(p_PingCompleted); ;

                p.SendAsync(ip, 1000, ip);
                _worker.ReportProgress((int)CalculateProgress(i, 255));
            }
        }

        private void p_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            string ip = (string)e.UserState;
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                string name;
                try
                {
                    IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                    name = hostEntry.HostName;
                }
                catch (SocketException ex)
                {
                    name = "?";
                }
                uiContext.Send(x => List.Add(new IPHostNameCombi() { IpAddress = ip, HostName = name }), null);

                Console.WriteLine("{0} ({1}) is up: ({2} ms)", ip, name, e.Reply.RoundtripTime);
            }
            else
            {
                Console.WriteLine("{0} is up: ({1} ms)", ip, e.Reply.RoundtripTime);
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

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) // if there is any subscribers 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
