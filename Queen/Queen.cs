using HiveSuite.Core;
using HiveSuite.Core.Network;
using HiveSuite.Queen.Queue;
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
        StorageHandler Storage { get; set; }

        public Queen()
        {
            Loging = new Logger();
            LoadSettings();
            Com = new ComHandler(Settings);
            Storage = new StorageHandler(Settings, new SQLiteStorage());
        }

        public override void MainLoop()
        {
            // Handle UDP Request
            NetworkMessage nextMsg = Com.ReadNext();

            if(nextMsg.Message == NetworkMessages.RequestTask.Message)
            {
                TaskData task = Storage.NextTask(nextMsg);
                Com.SendUDPMessage(NetworkMessages.ResponseTask(task), nextMsg);
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
