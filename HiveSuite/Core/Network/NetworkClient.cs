using HiveSuite.Drone;
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
    public class NetworkClient : NetworkBase
    {
        public NetPeerConfiguration Config { get; set; }
        protected static NetClient NetworkObj { get; set; }
        

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

        public NetworkClient(ISettings settings) : base()
        {
            Config = new NetPeerConfiguration("Hive");
            NetworkObj = new NetClient(Config);
            Settings = settings;

            ListenThread = new Thread(Listen);
            ListenThread.Start();

            NetworkObj.Start();
            NetOutgoingMessage hail = NetworkObj.CreateMessage("This is the hail message");
            NetworkObj.Connect(((DroneSettings)settings).ServerAddress, settings.Port, hail);
        }

        public void Listen()
        {
            NetIncomingMessage inMsg;

            while(true)
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
                            Messages.Enqueue(new NetworkMessage(inMsg.ReadString()));
                            break;
                        case NetIncomingMessageType.Receipt:
                            break;
                        case NetIncomingMessageType.DiscoveryRequest:
                            //NetOutgoingMessage tempMsg = NetworkObj.CreateMessage();
                            //NetworkObj.SendDiscoveryResponse(tempMsg, inMsg.SenderEndPoint);
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
        /// Sends a discovery message that will trigger connection to all other peers that respond
        /// </summary>
        public void SendDiscovery()
        {
            NetworkObj.DiscoverLocalPeers(Settings.Port);
            DateTime start = DateTime.Now;

            while(NetworkObj.ConnectionsCount == 0 && (DateTime.Now - start) < new TimeSpan(0,2,0))
            {
                Thread.Sleep(500);
            }

            if(NetworkObj.ConnectionsCount < 1)
            {
                throw new Exception("Could not find server to connect to");
            }
        }

        /// <summary>
        /// returns a bool True if the comms is up and false if the comms are down
        /// </summary>
        /// <returns></returns>
        public bool CommsUp()
        {
            if (ListenThread.IsAlive)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Send a message directly to the connected server
        /// </summary>
        /// <param name="msg">message to send</param>
        public void SendMessage(NetworkMessage msg)
        {
            NetConnection endPoint = NetworkObj.GetConnection(new IPEndPoint(IPAddress.Parse(((DroneSettings)Settings).ServerAddress), Settings.Port));

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
