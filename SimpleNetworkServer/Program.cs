using HiveSuite.Core;
using HiveSuite.Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleNetworkServer
{
    class Program
    {
        public static NetworkServer ComObject { get; set; }
        protected static Logger Log { get; set; }
        protected static ISettings Settings { get; set; }

        static void Main(string[] args)
        {
            Log = new Logger();

            ComObject = new NetworkServer(Settings);

            while(true)
            {
                Console.WriteLine(ComObject.PeerCount);
            }
        }
    }
}
