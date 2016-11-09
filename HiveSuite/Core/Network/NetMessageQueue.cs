using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiveSuite.Core.Network
{
    public class NetMessageQueue
    {
        /// <summary>
        /// thread locking for both Listen and Network
        /// </summary>
        private static ReaderWriterLockSlim QueueLock { get; set; }

        private LinkedList<NetworkMessage> Messages { get; set; }

        public NetMessageQueue()
        {
            Messages = new LinkedList<NetworkMessage>();
        }

        public NetworkMessage Dequeue()
        {
            QueueLock.EnterWriteLock();
            NetworkMessage result = Messages.First();
            Messages.RemoveFirst();
            QueueLock.ExitWriteLock();
            return result;
        }

        public void Enqueue(NetworkMessage msg)
        {
            QueueLock.EnterWriteLock();
            Messages.AddLast(msg);
            QueueLock.ExitWriteLock();
        }

        public NetworkMessage Peek()
        {
            QueueLock.EnterReadLock();
            NetworkMessage result = Messages.First();
            QueueLock.ExitReadLock();
            return result;
        }

        public List<NetworkMessage> Dump()
        {
            QueueLock.EnterWriteLock();
            List<NetworkMessage> result = Messages.ToList();
            Messages.Clear();
            QueueLock.ExitWriteLock();
            return result;
        }

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

        public void Remove(NetworkMessage toRemove)
        {
            QueueLock.EnterWriteLock();
            Messages.Remove(toRemove);
            QueueLock.ExitWriteLock();
        }

        public void Remove(int index)
        {
            QueueLock.EnterWriteLock();
            Messages.Remove(Messages.ElementAt(index));
            QueueLock.ExitWriteLock();
        }
    }
}
