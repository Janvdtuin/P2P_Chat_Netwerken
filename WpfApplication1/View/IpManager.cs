using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2P_Netwerken.View
{
    class IpManager
    {
        ObservableCollection<IPHostNameCombi> tl = new ObservableCollection<IPHostNameCombi>();
        internal ObservableCollection<IPHostNameCombi> GetCombis()
        {
            tl.Add(new IPHostNameCombi() { IpAddress = "State1", HostName = "Text1"});
            tl.Add(new IPHostNameCombi() { IpAddress = "State1", HostName = "Text1" });
            tl.Add(new IPHostNameCombi() { IpAddress = "State1", HostName = "Text1" });
            return tl;
        }
    }
}
