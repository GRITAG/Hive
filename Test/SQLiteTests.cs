using HiveSuite.Core;
using HiveSuite.Drone;
using HiveSuite.Queen.Queue;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Test
{
    [TestFixture]
    public class SQLiteTests
    {
        SQLiteStorage DB;
        TaskData TaskTest;

        [SetUp]
        public void Setup()
        {
            DB = new SQLiteStorage(new DroneSettings(new Core.Logger()));
            TaskTest = new TaskData
            {
                TaskID = Guid.NewGuid(),
                PackageID = Guid.NewGuid(),
                PackageHash = new byte[] { 1, 2, 3, 4, 5 },
                TaskFile = "test",
                Result = TaskResultType.Failed,
                Active = false,
                AssignedAddress = "1234567890"
            };
        }

        [Test]
        public void AddTask()
        {
            DB.AddTask(TaskTest);
            Assert.NotNull(DB.PeakTask(TaskTest.TaskID));
        }

        [Test]
        public void RemoveTask()
        {
            DB.AddTask(TaskTest);
            DB.RemoveTask(TaskTest.TaskID);
            Assert.IsNull(DB.PeakTask(TaskTest.TaskID));
        }
    }
}
