using HiveSuite.Core;
using HiveSuite.Core.Network;
using HiveSuite.Core.PackageObjects;
using HiveSuite.Drone;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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

            // load the test package
            byte[] testPackageFile = File.ReadAllBytes("TestPackage\\dir.zip");
            PackageTransmit testPackage = new PackageTransmit { Data = testPackageFile, ID = Guid.Empty };
            testPackage.MD5Hash = testPackage.GetMd5Hash();

            while (true)
            {
                NetworkMessage readyMsg = ComObject.PullMessage(NetworkMessages.Ready.Message);
                if (readyMsg != null)
                {
                    Console.WriteLine("Connect Request");
                    ComObject.SendMessage(NetworkMessages.AddToServer, IPAddress.Parse(readyMsg.SenderIP), readyMsg.SenderPort);
                    Console.WriteLine("Connecting Drone");
                }
                NetworkMessage requestTask = ComObject.PullMessage(NetworkMessages.RequestTask.Message);
                if (requestTask != null)
                {
                    Console.WriteLine("Task Request");
                    ComObject.SendMessage(NetworkMessages.ResponseTask(new TaskData()), IPAddress.Parse(requestTask.SenderIP), requestTask.SenderPort);
                    Console.WriteLine("Task Reponse sent");
                }
                NetworkMessage requestPackage = ComObject.PullMessage(NetworkMessages.RequestPackage(null).Message);
                if (requestPackage != null)
                {
                    Console.WriteLine("Package Request");
                    ComObject.SendMessage(NetworkMessages.ResponsePackage(testPackage), IPAddress.Parse(requestTask.SenderIP), requestTask.SenderPort);
                    Console.WriteLine("Package Reponse");
                }
                //Console.WriteLine(ComObject.PeerCount);
            }
        }
    }
}
