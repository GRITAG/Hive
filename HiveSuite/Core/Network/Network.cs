using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace HiveSuite.Core.Network
{
    /// <summary>
    /// The Network handling class for peer to peer coms
    /// thanks to: http://www.tylerforsythe.com/2011/11/peer-to-peer-networking-example-using-the-lidgren-network-framework/
    /// </summary>
    public class Network : NetworkBase
    {
        /// <summary>
        /// lidgern network object
        /// </summary>
        NetPeer NetworkConnctor;

        /// <summary>
        /// Listen thread object
        /// </summary>
        Listen ListenClass { get; set; }

        /// <summary>
        /// Listen thread
        /// </summary>
        Thread ListenThread { get; set; }

        /// <summary>
        /// Server address
        /// </summary>
        IPAddress Server { get; set; }

        /// <summary>
        /// server port
        /// </summary>
        int Port { get; set; }

        ISettings Settings { get; set; }

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

        /// <summary>
        /// Logging object, typiclay passed in
        /// </summary>
        Logger Logging { get; set; }

        /// <summary>
        /// Creates a network object
        /// </summary>
        /// <param name="port">the port to communicate on</param>
        /// <param name="log">Logging to use</param>
        public Network(int port, Logger log, ISettings settings)
        {
            Settings = settings;

            Port = port;

            Logging = log;

            Messages = new NetMessageQueue();
            

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

        /// <summary>
        /// creates a network object and automatilly connect to the server given
        /// </summary>
        /// <param name="server">ip address of the server</param>
        /// <param name="port">port to use with server communication</param>
        /// <param name="log">logging object to refrence</param>
        public Network(IPAddress server, int port, Logger log, ISettings settings) : this(port, log, settings)
        {
            Server = server;
            Port = port;

            NetworkConnctor.Connect(new IPEndPoint(Server, Port));
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

        /// <summary>
        /// Pull a message from the queue if it is there
        /// </summary>
        /// <param name="msgText">text of message to pull</param>
        /// <returns></returns>
        public NetworkMessage PullMessage(string msgText)
        {
            DateTime pullStart = DateTime.Now;
            NetworkMessage pullMessage = null;
            while(pullMessage  == null && pullStart < (pullStart + new TimeSpan(0,0,Settings.NetworkTimeout)))
            {
                pullMessage = Messages.Pull(msgText);
            }

            if(pullMessage == null)
            {
                throw new Exception("Pull for message timed out");
            }

            return pullMessage;
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

        /// <summary>
        /// returns a bool True if the comms is up and false if the comms are down
        /// </summary>
        /// <returns></returns>
        public bool CommsUp()
        {
            if(!ListenClass.CloseConnection && ListenThread.IsAlive)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Shutdown the network object and all related objects
        /// </summary>
        public void ShutdownNetworking()
        {
            ListenClass.CloseConnection = true;
        }

        /// <summary>
        /// Send a message blindly to any peer given
        /// </summary>
        /// <param name="msg">message to send</param>
        /// <param name="address">ip address of peer</param>
        /// <param name="port">port to use</param>
        public void SendMessage(NetworkMessage msg, IPAddress address, int port)
        {
            NetworkConnctor.SendMessage(NetworkConnctor.CreateMessage(msg.ToString()), NetworkConnctor.GetConnection(new IPEndPoint(address, port)), NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Send a message directly to the connected server
        /// </summary>
        /// <param name="msg">message to send</param>
        public void SendMessage(NetworkMessage msg)
        {
            NetworkConnctor.SendMessage(NetworkConnctor.CreateMessage(msg.ToString()), NetworkConnctor.GetConnection(new IPEndPoint(Server, Port)), NetDeliveryMethod.ReliableOrdered);
        }
    }

   
    

    
}
