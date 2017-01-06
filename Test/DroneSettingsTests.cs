using HiveSuite.Drone;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Test
{
    [TestFixture]
    public class DroneSettingsTests : UnitTest
    {
        DroneSettings Settings { get; set; }

        [SetUp]
        public void Setup()
        {
            Settings = new DroneSettings(new Core.Logger());
        }

        [TearDown]
        public void TearDown()
        {
            if(File.Exists(Directory.GetCurrentDirectory() + "\\settings.json"))
            {
                File.Delete(Directory.GetCurrentDirectory() + "\\settings.json");
            }
        }
        
        [Test]
        public void GenConfig()
        {
            Settings.GenerateConfig();
            Assert.IsTrue(File.Exists(Settings.DefaultFilePath));
        }

        [Test]
        public void LoadConfig()
        {
            Settings.GenerateConfig();
            Settings = null;
            Assert.IsNull(Settings);
            Settings = new DroneSettings(new Core.Logger());
            Settings.Load(Settings.DefaultFilePath);
            Assert.AreEqual(1000, Settings.Port);
            Assert.AreEqual("192.168.1.100", Settings.ServerAddress);
        }

        [Test]
        public void SaveConfig()
        {
            Settings.GenerateConfig();
            Settings = null;

            Settings = new DroneSettings(new Core.Logger());
            Assert.AreEqual(0, Settings.Port);
            Assert.AreEqual(string.Empty, Settings.ServerAddress);

            Settings.ServerAddress = "123456789";
            Settings.Port = 1;
            Settings.Save(Settings.DefaultFilePath);
            Settings = null;

            Settings = new DroneSettings(new Core.Logger());
            Settings.Load(Settings.DefaultFilePath);
            Assert.AreEqual(1, Settings.Port);
            Assert.AreEqual("123456789", Settings.ServerAddress);
        }

    }
}
