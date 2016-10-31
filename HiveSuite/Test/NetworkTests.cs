using HiveSuite.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Test
{
    [TestFixture]
    public class NetworkTests
    {
        Network Net1;
        Network Net2;

        [SetUp]
        public void Init()
        {
            Net1 = new Network();
        }

        [Test(Author ="Aaron V")]
        public void ThreadIsSpinning()
        {
            Assert.IsTrue(Net1.ListenThread.IsAlive);
        }

        [Test]
        public void Play()
        {
            Net1.LocalIP();
            Console.WriteLine("Test");
        }
    }
}
