using HiveSuite.Core;
using HiveSuite.Core.Network;
using HiveSuite.Drone;
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

            Settings = new DroneSettings(Log);
            Settings.NetworkTimeout = 60;

            ComObject = new NetworkServer(Settings);

            while(true)
            {
                NetworkMessage readyMsg = ComObject.PullMessage(NetworkMessages.Ready.Message);
                if(readyMsg != null)
                {
                    Console.WriteLine("Connect Request");
                    ComObject.SendMessage(NetworkMessages.AddToServer, IPAddress.Parse(readyMsg.SenderIP), readyMsg.SenderPort);
                    Console.WriteLine("Connecting Drone");
                }
                NetworkMessage requestTask = ComObject.PullMessage(NetworkMessages.RequestTask.Message);
                if(requestTask != null)
                {
                    Console.WriteLine("Task Request");
                    ComObject.SendMessage(NetworkMessages.ResponseTask(new TaskData()), IPAddress.Parse(requestTask.SenderIP), requestTask.SenderPort);
                    Console.WriteLine("Task Reponse sent");
                }
                //Console.WriteLine(ComObject.PeerCount);
            }
        }
    }
}
