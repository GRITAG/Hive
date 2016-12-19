using HiveSuite.Core;
using HiveSuite.Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Queen
{
    public class Queen : BaseNetworked
    {
        ComHandler Com { get; set; }

        public Queen()
        {
            Loging = new Logger();
            LoadSettings();
            Com = new ComHandler(Settings);
        }

        public override void MainLoop()
        {
            // Handle UDP Request
            NetworkMessage nextMsg = Com.ReadNext();

            if(nextMsg.Message == NetworkMessages.Ready.Message)
            {
                Com.SendUDPMessage(NetworkMessages.AddToServer, nextMsg);
            }


        }

        protected override bool InitilizeComms()
        {
            Com.StartWeb();
            return true;
        }

        protected override bool LoadSettings()
        {
            Settings = new QueenSettings();
            Settings.Load(Settings.DefaultFilePath);
            return true;
        }

        // network handler / web api
        // Web interface / requests though web api
        // Provisioning 
        // Queue management
        // Package Cache management 

        // Storage and Queue
        // External communication
        // Provisioning
        // Interface
    }
}
