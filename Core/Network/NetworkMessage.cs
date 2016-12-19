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
        /// <summary>
        /// message text 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// data needed for message (must be json serlizable)
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// The IP Address that sent the message
        /// </summary>
        public string SenderIP { get; set; }

        /// <summary>
        /// The port the message was sent on
        /// </summary>
        public int SenderPort { get; set; }

        public NetworkMessage() { }

        /// <summary>
        /// Creates a mnetwork message object
        /// </summary>
        /// <param name="msg">message text</param>
        /// <param name="data">any data need to be sent (must be json serlizable)</param>
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
