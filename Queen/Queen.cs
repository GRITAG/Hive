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
            Storage = new StorageHandler(Settings, new SQLiteStorage(Settings));
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
                        if (task != null)
                        {
                            Com.SendUDPMessage(NetworkMessages.ResponseTask(task), nextMsg);
                        }
                        else
                        {
                            Com.SendUDPMessage(NetworkMessages.ReponseNoTask(), nextMsg);
                        }
                    }
                    else if(nextMsg.Message == NetworkMessages.ReadyMessage)
                    {
                        Com.SendUDPMessage(NetworkMessages.AddToServer, nextMsg);
                    }
                    else if(nextMsg.Message == NetworkMessages.RequestPackageMessge)
                    {
                        PackageData packageInfo = (PackageData)nextMsg.Data;
                        Package packageFound = Storage.GetPackage(packageInfo.ID, packageInfo.MD5Hash);
                        if(packageFound != null)
                        {

                        }
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
