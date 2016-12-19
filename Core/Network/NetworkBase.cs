using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiveSuite.Core.Network
{
    /// <summary>
    /// Base network object used to share thread locking and messages between 
    /// network object and listen thread
    /// </summary>
    public class NetworkBase
    {
        /// <summary>
        /// Settings file ref
        /// </summary>
        protected ISettings Settings { get; set; }
        /// <summary>
        /// Thread used for process / task execution 
        /// </summary>
        protected Thread ListenThread { get; set; }
        
        /// <summary>
        /// application accessable network messages
        /// </summary>
        public static NetMessageQueue Messages { get; set; }

        public NetworkBase()
        {
            Messages = new NetMessageQueue();
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
            while (pullMessage == null && (DateTime.Now - pullStart) < new TimeSpan(0, 0, Settings.NetworkTimeout))
            {
                pullMessage = Messages.Pull(msgText);
            }

            if (pullMessage == null)
            {
                //throw new Exception("Pull for message timed out");
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

        
    }
}
