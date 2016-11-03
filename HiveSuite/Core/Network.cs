using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiveSuite.Core
{
    /// <summary>
    /// The Network handling class for peer to peer coms
    /// thanks to: http://www.tylerforsythe.com/2011/11/peer-to-peer-networking-example-using-the-lidgren-network-framework/
    /// </summary>
    public class Network
    {
        NetPeer NetworkConnctor;

        Queue<NetworkMessage> Messages { get; set; }

        ReaderWriterLockSlim QueueLock { get; set; }

        Listen ListenClass { get; set; }

        Thread ListenThread { get; set; }

        int Port { get; set; }

        public List<NetConnection> Peers
        {
            get
            {
                return NetworkConnctor.Connections;
            }
        }

        public int PeerCount
        {
            get
            {
                return NetworkConnctor.ConnectionsCount;
            }
        }

        Logger Logging { get; set; }

        public Network(int port, Logger log)
        {
            Port = port;

            Logging = log;

            Messages = new Queue<NetworkMessage>();
            QueueLock = new ReaderWriterLockSlim();

            NetPeerConfiguration config = new NetPeerConfiguration("Swarm")
            {
                LocalAddress = NetUtility.Resolve("localhost")
            };

            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.EnableMessageType(NetIncomingMessageType.UnconnectedData);

            NetworkConnctor = new NetPeer(config);
            NetworkConnctor.Start();

            ListenClass = new Listen(NetworkConnctor, this, log);
            ListenThread = new Thread(ListenClass.Loop);
            ListenThread.Start();
        }

        public NetworkMessage ReadMessage()
        {
            QueueLock.EnterReadLock();
            NetworkMessage nextMessage = Messages.Dequeue();
            QueueLock.ExitReadLock();

            return nextMessage;
        }

        public List<NetworkMessage> ReadMessages()
        {
            QueueLock.EnterReadLock();
            List<NetworkMessage> nextMessages = Messages.ToList();
            Messages.Clear();
            QueueLock.ExitReadLock();

            return nextMessages;
        }

        public void AddMessage(NetworkMessage toAdd)
        {
            QueueLock.EnterReadLock();
            Messages.Enqueue(toAdd);
            QueueLock.ExitReadLock();
        }

        public void SendDiscovery()
        {
            NetworkConnctor.DiscoverLocalPeers(Port);
        }

        public void SendPeerInfo(IPAddress ip, int port)
        {
            NetOutgoingMessage msg = NetworkConnctor.CreateMessage();
            msg.Write(MessageType.PeerInfo);
            byte[] addressBytes = ip.GetAddressBytes();
            msg.Write(addressBytes.Length);
            msg.Write(addressBytes);
            msg.Write(port);
            NetworkConnctor.SendMessage(msg, NetworkConnctor.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public IPAddress LocalIP()
        {
            return NetworkConnctor.Configuration.LocalAddress;
        }

        public bool CommsUp()
        {
            if(!ListenClass.CloseConnection && ListenThread.IsAlive)
            {
                return true;
            }

            return false;
        }

        public void ShutdownNetworking()
        {
            ListenClass.CloseConnection = true;
        }
    }

    public class Listen
    {
        NetPeer PeerRef;
        Network NetRef;

        public bool CloseConnection = false;

        Logger Logging;

        public Listen(NetPeer peer, Network net, Logger log)
        {
            PeerRef = peer;
            NetRef = net;
            Logging = log;
        }

        public void Loop()
        {
            while (!CloseConnection)
            {
                Thread.Sleep(1);
                if (PeerRef == null)
                    continue;
                NetIncomingMessage msg;
                while ((msg = PeerRef.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.UnconnectedData:
                            string orphanData = msg.ReadString();
                            Logging.Log(LogLevel.Error,"UnconnectedData: " + orphanData);
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                            msg.SenderConnection.Approve();
                            NetRef.SendPeerInfo(msg.SenderEndPoint.Address, msg.SenderEndPoint.Port);
                            break;
                        case NetIncomingMessageType.Data:
                            switch (msg.ReadString())
                            {
                                case MessageType.String:
                                    break;
                                case MessageType.PeerInfo:
                                    Logging.Log(LogLevel.Info, "Data::PeerInfo BEGIN");
                                    int byteLenth = msg.ReadInt32();
                                    byte[] addressBytes = msg.ReadBytes(byteLenth);
                                    IPAddress ip = new IPAddress(addressBytes);
                                    int port = msg.ReadInt32();
                                    //connect
                                    IPEndPoint endPoint = new IPEndPoint(ip, port);
                                    Logging.Log(LogLevel.Info, "Data::PeerInfo::Detecting if we're connected");
                                    if (PeerRef.GetConnection(endPoint) == null)
                                    {//are we already connected?
                                        //Don't try to connect to ourself!
                                        if (PeerRef.Configuration.LocalAddress.GetHashCode() != endPoint.Address.GetHashCode()
                                                || PeerRef.Configuration.Port.GetHashCode() != endPoint.Port.GetHashCode())
                                        {
                                            Logging.Log(LogLevel.Info, string.Format("Data::PeerInfo::Initiate new connection to: {0}:{1}",
                                                endPoint.Address.ToString(), endPoint.Port.ToString()));
                                            PeerRef.Connect(endPoint);
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case NetIncomingMessageType.DiscoveryRequest:
                            PeerRef.SendDiscoveryResponse(null, msg.SenderEndPoint);
                            break;
                        case NetIncomingMessageType.DiscoveryResponse:
                            PeerRef.Connect(msg.SenderEndPoint);
                            break;
                        default:
                            Logging.Log(LogLevel.Error, "ReceivePeersData Unknown type: " + msg.MessageType.ToString());
                            try
                            {
                                Console.WriteLine(msg.ReadString());
                            }
                            catch
                            {
                                Logging.Log(LogLevel.Error, "Couldn't parse unknown to string.");
                            }
                            break;
                    }
                }
            }
        }
    }

    public class MessageType
    {
        public const string Json = "Json";
        public const string String = "String";
        public const string Binary = "Binary";
        public const string PeerInfo = "PeerInfo";
    }
}
