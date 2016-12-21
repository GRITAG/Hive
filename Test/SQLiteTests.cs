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
        TaskData TaskTest1;
        TaskData TaskTest2;

        [SetUp]
        public void Setup()
        {
            DB = new SQLiteStorage(new DroneSettings(new Core.Logger()));

            TaskTest1 = new TaskData
            {
                TaskID = Guid.NewGuid(),
                PackageID = Guid.NewGuid(),
                PackageHash = new byte[] { 1, 2, 3, 4, 5 },
                TaskFile = "test",
                Result = TaskResultType.Failed,
                Active = false,
                AssignedAddress = "1234567890"
            };

            TaskTest2 = new TaskData
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

        [TearDown]
        public void TearDown()
        {
            foreach(TaskData currentTask in DB.PeakAllTasks())
            {
                DB.RemoveTask(currentTask.TaskID);
            }
        }

        [Test]
        public void AddTask()
        {
            DB.AddTask(TaskTest1);
            Assert.NotNull(DB.PeakTask(TaskTest1.TaskID));
        }

        [Test]
        public void RemoveTask()
        {
            DB.AddTask(TaskTest1);
            DB.RemoveTask(TaskTest1.TaskID);
            Assert.IsNull(DB.PeakTask(TaskTest1.TaskID));
        }

        [Test]
        public void PeakAllTasks()
        {
            DB.AddTask(TaskTest1);
            DB.AddTask(TaskTest2);
            Assert.AreEqual(2, DB.PeakAllTasks().Count);
        }
    }
}
