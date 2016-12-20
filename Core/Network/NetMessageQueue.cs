using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HiveSuite.Core.Network
{
    /// <summary>
    /// Storage object for network messages built as a First in First out Queue
    /// </summary>
    public class NetMessageQueue
    {
        public int Count
        {
            get
            {
                int returnCount = 0;

                QueueLock.EnterReadLock();
                returnCount = Messages.Count;
                QueueLock.ExitReadLock();

                return returnCount;
            }
        }

        /// <summary>
        /// thread locking for both Listen and Network
        /// </summary>
        private static ReaderWriterLockSlim QueueLock { get; set; }

        /// <summary>
        /// Collection of network messages taken in from the external network
        /// </summary>
        private LinkedList<NetworkMessage> Messages { get; set; }


        public NetMessageQueue()
        {
            Messages = new LinkedList<NetworkMessage>();
            QueueLock = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Take the first message on the stack (bottem first)
        /// </summary>
        /// <returns></returns>
        public NetworkMessage Dequeue()
        {
            QueueLock.EnterWriteLock();
            NetworkMessage result = null;
            if (Messages.Count > 1)
            {
                result = Messages.First();
                Messages.RemoveFirst();
            }
            QueueLock.ExitWriteLock();
            return result;
        }

        /// <summary>
        /// Add a new message to the bottem of the queue
        /// </summary>
        /// <param name="msg"></param>
        public void Enqueue(NetworkMessage msg)
        {
            QueueLock.EnterWriteLock();
            Messages.AddLast(msg);
            QueueLock.ExitWriteLock();
        }

        /// <summary>
        /// Peak at the first message in the queue (will not dequeue)
        /// </summary>
        /// <returns></returns>
        public NetworkMessage Peek()
        {
            QueueLock.EnterReadLock();
            NetworkMessage result = Messages.First();
            QueueLock.ExitReadLock();
            return result;
        }

        /// <summary>
        /// pull all messages of the queue and return them
        /// </summary>
        /// <returns>complete list of network messages</returns>
        public List<NetworkMessage> Dump()
        {
            QueueLock.EnterWriteLock();
            List<NetworkMessage> result = Messages.ToList();
            Messages.Clear();
            QueueLock.ExitWriteLock();
            return result;
        }

        /// <summary>
        /// Pull a message from the queue if it matches the message text given(dequeues message pulled)
        /// </summary>
        /// <param name="messageText">message text to serach for</param>
        /// <returns></returns>
        public NetworkMessage Pull(string messageText)
        {
            NetworkMessage result = null;

            foreach (NetworkMessage currentMsg in Messages)
            {
                if(currentMsg.Message == messageText)
                {
                    result = currentMsg;
                }
            }

            if(result != null)
            {
                Remove(result);
            }

            return result;
        }

        /// <summary>
        /// remove a message from the queue using an object
        /// </summary>
        /// <param name="toRemove">object to remove</param>
        public void Remove(NetworkMessage toRemove)
        {
            QueueLock.EnterWriteLock();
            Messages.Remove(toRemove);
            QueueLock.ExitWriteLock();
        }

        /// <summary>
        /// remove a message from the queue using an index
        /// </summary>
        /// <param name="index">index of the object to remove</param>
        public void Remove(int index)
        {
            QueueLock.EnterWriteLock();
            Messages.Remove(Messages.ElementAt(index));
            QueueLock.ExitWriteLock();
        }

        
    }
}
