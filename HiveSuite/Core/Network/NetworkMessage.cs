using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Core.Network
{
    /// <summary>
    /// Messging object used to send data between drones and server
    /// </summary>
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

        /// <summary>
        /// converts json string to a network message
        /// </summary>
        /// <param name="json">json formated string</param>
        public NetworkMessage(string json)
        {
            NetworkMessage temp = new NetworkMessage();
            temp = JsonConvert.DeserializeObject<NetworkMessage>(json);

            Message = temp.Message;
            Data = temp.Data;
        }

        /// <summary>
        /// Override used to retrieve a json string formated network message
        /// </summary>
        /// <returns>Json formated network object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
