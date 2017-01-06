using HiveSuite.Core;
using HiveSuite.Core.Network;
using HiveSuite.Queen.PackageObjects;
using HiveSuite.Queen.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Queen
{
    public class StorageHandler
    {
        ISettings Settings { get; set; }
        IQueueStorage Queue { get; set; }
        PackageHandler Packages { get; set; }

        public StorageHandler(ISettings settings, IQueueStorage queue)
        {
            Settings = settings;
            Queue = queue;
            Packages = new PackageHandler(Settings);
        }

        public TaskData NextTask(NetworkMessage incomingMsg)
        {
            return Queue.PullNextTask(incomingMsg);
        }

        public Package GetPackage(Guid id, byte[] md5)
        {
            return Packages.GetPackage(id, md5);
        }
    }
}
