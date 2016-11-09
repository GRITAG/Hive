using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiveSuite.Core.Network
{
    /// <summary>
    /// Base network object used to share thread locking and messages between 
    /// network object and listen thread
    /// </summary>
    public class NetworkBase
    {
        /// <summary>
        /// application accessable network messages
        /// </summary>
        public static NetMessageQueue Messages { get; set; }
    }
}
