using HiveSuite.Core.PackageObjects;
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

        public static NetworkMessage ResponseTask(object data)
        {
            return new NetworkMessage("Task Response", data);
        }

        public static NetworkMessage RequestPackage(Guid id, byte[] md5Hash)
        {
            return RequestPackage(new PackageData
            {
                ID = id,
                MD5Hash = md5Hash
            });
        }

        public static NetworkMessage RequestPackage(PackageData data)
        {
            return new NetworkMessage("Package Request", data);
        }

        public static NetworkMessage ResponsePackage(PackageTransmit data)
        {
            return new NetworkMessage("Pacakge Response", data);
        }
    }
}
