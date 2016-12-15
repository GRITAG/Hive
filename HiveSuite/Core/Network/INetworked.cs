using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiveSuite.Core.Network
{
    interface INetworked
    {
        NetPeerConfiguration Config { get; set; }
        ISettings Settings { get; set; }
        Thread ListenThread { get; set; }
        int PeerCount { get; }


        NetworkMessage ReadMessage();
        List<NetworkMessage> ReadMessages();
        NetworkMessage PullMessage(string msgText);
        void AddMessage(NetworkMessage toAdd);
    }
}
