using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiveSuite.Core.Network
{
    public class NetworkServer : NetworkBase
    {
        NetPeerConfiguration Config { get; set; }
        static NetServer NetworkObj { get; set; }
        Thread ListenThread { get; set; }

        /// <summary>
        /// Returns connected peers
        /// </summary>
        public List<NetConnection> Peers
        {
            get
            {
                return NetworkObj.Connections;
            }
        }

        /// <summary>
        /// Returns the count of connected peers
        /// </summary>
        public int PeerCount
        {
            get
            {
                return NetworkObj.ConnectionsCount;
            }
        }

        public NetworkServer(ISettings settings) : base()
        {
            Config = new NetPeerConfiguration("Hive");
            Config.MaximumConnections = 1000;
            Config.Port = 1000;
            NetworkObj = new NetServer(Config);
            Settings = settings;
            NetworkObj.Start();

            ListenThread = new Thread(Listen);
            ListenThread.Start();
        }

        public static void Listen()
        {
            NetIncomingMessage inMsg;

            while (true)
            {
                inMsg = NetworkObj.ReadMessage();

                if (inMsg != null)
                {

                    switch (inMsg.MessageType)
                    {
                        case NetIncomingMessageType.Error:
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            break;
                        case NetIncomingMessageType.UnconnectedData:
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                            inMsg.SenderConnection.Approve();
                            break;
                        case NetIncomingMessageType.Data:
                            NetworkMessage tempMsg = new NetworkMessage(inMsg.ReadString());
                            tempMsg.SenderIP = inMsg.SenderEndPoint.Address.ToString();
                            tempMsg.SenderPort = inMsg.SenderEndPoint.Port;
                            Messages.Enqueue(tempMsg);
                            break;
                        case NetIncomingMessageType.Receipt:
                            break;
                        case NetIncomingMessageType.DiscoveryRequest:
                            NetOutgoingMessage tempMsg2 = NetworkObj.CreateMessage();
                            NetworkObj.SendDiscoveryResponse(tempMsg2, inMsg.SenderEndPoint);
                            break;
                        case NetIncomingMessageType.DiscoveryResponse:
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                            break;
                        case NetIncomingMessageType.DebugMessage:
                            break;
                        case NetIncomingMessageType.WarningMessage:
                            break;
                        case NetIncomingMessageType.ErrorMessage:
                            break;
                        case NetIncomingMessageType.NatIntroductionSuccess:
                            break;
                        case NetIncomingMessageType.ConnectionLatencyUpdated:
                            break;
                        default:
                            break;
                    }
                }

                NetworkObj.Recycle(inMsg);
            }
        }

        /// <summary>
        /// Send a message directly to the connected server
        /// </summary>
        /// <param name="msg">message to send</param>
        public void SendMessage(NetworkMessage msg, IPAddress endPointAddress, int port)
        {
            NetConnection endPoint = NetworkObj.GetConnection(new IPEndPoint(endPointAddress, port));

            if (endPoint != null)
            {
                NetworkObj.SendMessage(NetworkObj.CreateMessage(msg.ToString()), endPoint, NetDeliveryMethod.ReliableOrdered);
            }
            else
            {
                throw new Exception("No end point to communicate with");
            }
        }
    }

    
}
