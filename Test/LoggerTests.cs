using HiveSuite.Core;
using NUnit.Framework;
using System.IO;

namespace HiveSuite.Test
{
    [TestFixture(Category ="Logging")]
    public class LoggerTests : UnitTest
    {
        Logger Log { get; set; }

        [SetUp]
        public void Setup()
        {
            Log = new Logger();
        }

        [Test]
        public void LogMessage()
        {
            string TextToWrite = "The Ruff...The Ruff..The Ruff is on fire";
            Log.Log(LogLevel.Error, TextToWrite);
            Assert.IsTrue(File.Exists(TestContext.CurrentContext.TestDirectory + "\\Application.log"));
            string logContents =  File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\Application.log");
            Assert.IsTrue(logContents.Contains(TextToWrite));
            File.Delete(TestContext.CurrentContext.TestDirectory + "\\Application.log");
        }
    }
}
