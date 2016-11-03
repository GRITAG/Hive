using HiveSuite.Core;
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
    public class NetworkTests
    {
        Network Net1;
        Network Net2;

        [SetUp]
        public void Init()
        {
            Net1 = new Network(1, null);
        }

        [Test(Author ="Aaron V")]
        public void ThreadIsSpinning()
        {
            Assert.IsTrue(Net1.CommsUp());
        }

        [Test]
        public void Play()
        {
            Net1.LocalIP();
            Console.WriteLine("Test");
        }
    }
}
