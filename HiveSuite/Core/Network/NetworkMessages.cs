using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Core.Network
{
    /// <summary>
    /// Simple storage of commonly used network messages
    /// </summary>
    public class NetworkMessages
    {
        /// <summary>
        /// requests a task from the server
        /// </summary>
        public static NetworkMessage RequestTask = new NetworkMessage { Message ="Task Requested", Data = null } ;
    }
}
