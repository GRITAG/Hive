using HiveSuite.Core;
using HiveSuite.Core.Network;
using HiveSuite.Queen.Web;
using System;
using System.Net;

namespace HiveSuite.Queen
{
    public class ComHandler
    {
        protected WebHandler Web { get; set; }
        protected NetworkServer NetServer { get; set; }

        public ComHandler(ISettings settings)
        {
            // Must Start web server after registering API Calls
            Web = new WebHandler(settings);
            NetServer = new NetworkServer(settings);
        }

        public void RegesterAPIController(Type controllerType)
        {
            Web.RegisterAPIController(controllerType);
        }

        public void StartWeb()
        {
            Web.Start();
        }

        public void SendUDPMessage(NetworkMessage msg, IPAddress address, int port)
        {
            NetServer.SendMessage(msg, address, port);
        }

        public void SendUDPMessage(NetworkMessage msg, NetworkMessage oldMsg)
        {
            SendUDPMessage(msg, IPAddress.Parse(oldMsg.SenderIP), oldMsg.SenderPort);
        }

        public NetworkMessage ReadNext()
        {
            NetworkMessage currentMsg = NetServer.ReadMessage();

            if(currentMsg.Message == NetworkMessages.Ready.Message)
            {
                SendUDPMessage(NetworkMessages.AddToServer, currentMsg);
                return null;
            }

            return currentMsg;
        }
    }
}
