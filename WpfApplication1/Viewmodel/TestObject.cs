using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2P_Netwerken.Viewmodel
{
    public sealed class TestObject
    {
        public string test { get; set; }

        TestObject()
        {
        }
        private static readonly object padlock = new object();
        private static TestObject instance = null;
        public static TestObject Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new TestObject();
                    }
                    return instance;
                }
            }
        }
    }
}
