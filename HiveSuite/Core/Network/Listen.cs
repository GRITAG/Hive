using Lidgren.Network;
using System;
using System.Net;
using System.Threading;

namespace HiveSuite.Core.Network
{
    public class Listen : NetworkBase
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
                            Logging.Log(LogLevel.Error, "UnconnectedData: " + orphanData);
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                            msg.SenderConnection.Approve();
                            NetRef.SendPeerInfo(msg.SenderEndPoint.Address, msg.SenderEndPoint.Port);
                            break;
                        case NetIncomingMessageType.Data:
                            switch (msg.ReadString())
                            {
                                case MessageType.String:
                                    Messages.Enqueue(new NetworkMessage(msg.ReadString()));
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

}
