using HiveSuite.Core.PackageObjects;
using HiveSuite.Drone;
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
        #region Drone

        public static NetworkMessage Ready = new NetworkMessage { Message = ReadyMessage };

        /// <summary>
        /// requests a task from the server
        /// </summary>
        public static NetworkMessage RequestTask = new NetworkMessage { Message = RequestTaskMessage };

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
            return new NetworkMessage(RequestPackageMessge, data);
        }

        public static NetworkMessage StatusReponse(Status data)
        {
            return new NetworkMessage(StatusResponseMessage, data);
        }

        public static NetworkMessage ErrorReport(LogLevel level, string msg)
        {
            return new NetworkMessage(ErrorMessage, new NLog.LogEventInfo(Logger.TranslateLogLevel(level), "Hive", msg));
        }

        public static NetworkMessage TaskComplete(Guid id)
        {
            return new NetworkMessage(TaskCompleteMessage, id);
        }
        #endregion

        #region Queen
        public static NetworkMessage AddToServer = new NetworkMessage { Message = AddedToServerMessage };
        
        public static NetworkMessage ResponseTask(object data)
        {
            return new NetworkMessage(RequestTaskMessage, data);
        }

        public static NetworkMessage ResponsePackage(PackageTransmit data)
        {
            return new NetworkMessage(ResponsePackageMessage, data);
        }

        public static NetworkMessage StatusReceived = new NetworkMessage { Message = StatusResponseMessage };

        public static NetworkMessage ErrorReceived = new NetworkMessage { Message = ErrorReceivedMessage };

        public static NetworkMessage TaskCompleteReceived = new NetworkMessage { Message = TaskCompleteReceivedMessage };
        #endregion

        #region Messages
        // Drone
        public const string ReadyMessage = "ready";
        public const string RequestTaskMessage = "Task Requested";
        public const string RequestPackageMessge = "Package Request";
        public const string StatusResponseMessage = "Status";
        public const string ErrorMessage = "Error";
        public const string TaskCompleteMessage = "Task Complete";

        // Queen
        public const string AddedToServerMessage = "Added to Server";
        public const string ResponseTaskMessage = "Task Response";
        public const string ResponsePackageMessage = "Package Response";
        public const string StatusReceivedMessage = "Status Received";
        public const string ErrorReceivedMessage = "Error Received";
        public const string TaskCompleteReceivedMessage = "Task Complete Received";
        #endregion
    }
}
