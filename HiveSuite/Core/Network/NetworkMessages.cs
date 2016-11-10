using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Core.Network
{
    public class NetworkMessages
    {
        public static NetworkMessage RequestTask = new NetworkMessage { Message ="Task Requested", Data = null } ;
    }
}
