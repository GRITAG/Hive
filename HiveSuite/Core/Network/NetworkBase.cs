using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiveSuite.Core.Network
{
    public class NetworkBase
    {
        public static Queue<NetworkMessage> Messages { get; set; }

        public static ReaderWriterLockSlim QueueLock { get; set; }
    }
}
