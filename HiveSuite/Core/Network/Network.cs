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

        /// <summary>
        /// Returns connected peers
        /// </summary>
        public List<NetConnection> Peers
        {
            get
            {
                return NetworkConnctor.Connections;
            }
        }

        /// <summary>
        /// Returns the count of connected peers
        /// </summary>
        public int PeerCount
        {
            get
            {
                return NetworkConnctor.ConnectionsCount;
            }
        }

        Logger Logging { get; set; }

        /// <summary>
        /// Creates a network object
        /// </summary>
        /// <param name="port">the port to communicate on</param>
        /// <param name="log">Logging to use</param>
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

        public Network(IPAddress server, int port, Logger log) : this(port, log)
        {
            NetworkConnctor.Connect(new IPEndPoint(server, port));
        }

        /// <summary>
        /// Reads a NetworkMessage from the top of the stack (dequeues)
        /// </summary>
        /// <returns>Network Message</returns>
        public NetworkMessage ReadMessage()
        {
            return Messages.Dequeue();
        }

        /// <summary>
        /// Returns all current network messages (dequeues)
        /// </summary>
        /// <returns>List of NetworkMessages</returns>
        public List<NetworkMessage> ReadMessages()
        {
            return Messages.Dump();
        }

        public NetworkMessage PullMessage(string msgText)
        {
            return Messages.Pull(msgText);
        }

        /// <summary>
        /// Adds a Network Message directly to the queue
        /// </summary>
        /// <param name="toAdd">message to add</param>
        public void AddMessage(NetworkMessage toAdd)
        {
            Messages.Enqueue(toAdd);
        }

        /// <summary>
        /// Sends a discovery message that will trigger connection to all other peers that respond
        /// </summary>
        public void SendDiscovery()
        {
            NetworkConnctor.DiscoverLocalPeers(Port);
        }

        /// <summary>
        /// Sends peer info to the end point that should cause connection
        /// </summary>
        /// <param name="ip">ip end point of the peer</param>
        /// <param name="port">port to connect to</param>
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

        /// <summary>
        /// return the current ip used for c
        /// </summary>
        /// <returns></returns>
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
