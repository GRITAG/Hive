using HiveSuite.Core.Network;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Test
{
    [TestFixture]
    public class NetMessageQueueTests
    {
        NetMessageQueue Queue { get; set; }

        [SetUp]
        public void Setup()
        {
            Queue = new NetMessageQueue();
        }

        private void EnqueueThree()
        {
            Queue.Enqueue(new NetworkMessage
            {
                Message = "Message1",
                Data = null
            });

            Queue.Enqueue(new NetworkMessage
            {
                Message = "Message2",
                Data = null
            });

            Queue.Enqueue(new NetworkMessage
            {
                Message = "Message3",
                Data = null
            });
        }

        [Test(Author = "Aaron V")]
        public void Enqueue()
        {
            EnqueueThree();

            Assert.AreEqual(3, Queue.Count);
        }

        [Test(Author = "Aaron V")]
        public void Dequeue()
        {
            EnqueueThree();

            NetworkMessage result = Queue.Dequeue();

            Assert.AreEqual("Message1", result.Message);
            Assert.AreEqual(2, Queue.Count);
        }

        [Test(Author = "Aaron V")]
        public void Peak()
        {
            EnqueueThree();
            NetworkMessage peakMsg = Queue.Peek();

            Assert.AreEqual("Message1", peakMsg.Message);
            Assert.AreEqual(3, Queue.Count);
        }

        [Test(Author = "Aaron V")]
        public void Dump()
        {
            EnqueueThree();
            Assert.AreEqual(3, Queue.Count);

            List<NetworkMessage> dumpResult = Queue.Dump();
            Assert.AreEqual(3, dumpResult.Count);
            Assert.AreEqual(0, Queue.Count);
        }

        [Test(Author = "Aaron V")]
        public void Pull()
        {
            EnqueueThree();

            NetworkMessage pulledMsg = Queue.Pull("Message2");
            Assert.NotNull(pulledMsg);
            Assert.AreEqual(2, Queue.Count);
        }

        [Test(Author = "Aaron V")]
        public void RemoveObject()
        {
            EnqueueThree();
            Queue.Remove(Queue.Peek());
            Assert.AreEqual(2, Queue.Count);
            Assert.IsNull(Queue.Pull("Message1"));
        }

        [Test(Author = "Aaron V")]
        public void RemoveIndex()
        {
            EnqueueThree();
            Queue.Remove(2);
            Assert.AreEqual(2, Queue.Count);
            Assert.IsNull(Queue.Pull("Message3"));
        }
    }
}
