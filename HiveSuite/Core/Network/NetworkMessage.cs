using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Core.Network
{
    public class NetworkMessage
    {
        public string Message { get; set; }
        public object Data { get; set; }

        public NetworkMessage() { }

        public NetworkMessage(string msg, object data)
        {
            Message = msg;
            Data = data;
        }

        public NetworkMessage(string json)
        {
            NetworkMessage temp = new NetworkMessage();
            temp = JsonConvert.DeserializeObject<NetworkMessage>(json);

            Message = temp.Message;
            Data = temp.Data;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
