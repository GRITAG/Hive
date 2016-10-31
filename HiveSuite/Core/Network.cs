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

        public Listen ListenClass { get; set; }

        public Thread ListenThread { get; set; }

        public Network()
        {
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

            ListenClass = new Listen(NetworkConnctor, this);
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
    }

    public class Listen
    {
        NetPeer PeerRef;
        Network NetRef;

        public bool CloseConnection = false;

        public Listen(NetPeer peer, Network net)
        {
            PeerRef = peer;
            NetRef = net;
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
                            Console.WriteLine("UnconnectedData: " + orphanData);
                            break;
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
                                    Console.WriteLine("Data::PeerInfo BEGIN");
                                    int byteLenth = msg.ReadInt32();
                                    byte[] addressBytes = msg.ReadBytes(byteLenth);
                                    IPAddress ip = new IPAddress(addressBytes);
                                    int port = msg.ReadInt32();
                                    //connect
                                    IPEndPoint endPoint = new IPEndPoint(ip, port);
                                    Console.WriteLine("Data::PeerInfo::Detecting if we're connected");
                                    if (PeerRef.GetConnection(endPoint) == null)
                                    {//are we already connected?
                                        //Don't try to connect to ourself!
                                        if (PeerRef.Configuration.LocalAddress.GetHashCode() != endPoint.Address.GetHashCode()
                                                || PeerRef.Configuration.Port.GetHashCode() != endPoint.Port.GetHashCode())
                                        {
                                            Console.WriteLine(string.Format("Data::PeerInfo::Initiate new connection to: {0}:{1}",
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
                            Console.WriteLine("ReceivePeersData Unknown type: " + msg.MessageType.ToString());
                            try
                            {
                                Console.WriteLine(msg.ReadString());
                            }
                            catch
                            {
                                Console.WriteLine("Couldn't parse unknown to string.");
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
