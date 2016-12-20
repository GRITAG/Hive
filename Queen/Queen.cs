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

        bool Shutdown = false;

        public Queen()
        {
            Settings = new QueenSettings(Loging);
            Loging = new Logger();
            Com = new ComHandler(Settings);
            Storage = new StorageHandler(Settings, new SQLiteStorage());
        }

        public override void MainLoop()
        {
            Settings.Load(Settings.DefaultFilePath);
            Com.StartWeb();

            while (!Shutdown)
            {
                #region NetworkMsgResolve
                // Handle UDP Request
                NetworkMessage nextMsg = Com.ReadNext();

                if (nextMsg != null)
                {
                    if (nextMsg.Message == NetworkMessages.RequestTaskMessage)
                    {
                        TaskData task = Storage.NextTask(nextMsg);
                        Com.SendUDPMessage(NetworkMessages.ResponseTask(task), nextMsg);
                    }
                }
                #endregion

                // TODO: Validate Drones / Clean Up
                // TODO: Re-queue Work if valid
            }

        }

        protected override bool InitilizeComms()
        {
            Com.StartWeb();
            return true;
        }

        protected override bool LoadSettings()
        {
            Settings = new QueenSettings(Loging);
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
