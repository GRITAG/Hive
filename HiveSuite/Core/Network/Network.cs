using Lidgren.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiveSuite.Core.Network
{
    /// <summary>
    /// The Network handling class for peer to peer coms
    /// thanks to: http://www.tylerforsythe.com/2011/11/peer-to-peer-networking-example-using-the-lidgren-network-framework/
    /// </summary>
    public class Network : NetworkBase
    {
        NetPeer NetworkConnctor;

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

        public void SendMessage(NetworkMessage msg, IPAddress address, int port)
        {
            NetworkConnctor.SendMessage(NetworkConnctor.CreateMessage(msg.ToString()), NetworkConnctor.GetConnection(new IPEndPoint(address, port)), NetDeliveryMethod.ReliableOrdered);
        }
    }

   
    

    
}
