using HiveSuite.Core;
using HiveSuite.Core.Network;
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
    [TestFixture(Category ="SQLite")]
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
            foreach (TaskData currentTask in DB.PeakAllTasks())
            {
                DB.RemoveTask(currentTask.TaskID);
            }

            DB.Close();
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

        [Test]
        public void TaskCount()
        {
            DB.AddTask(TaskTest1);
            DB.AddTask(TaskTest2);
            Assert.AreEqual(2, DB.TaskCount());
        }

        [Test]
        public void PeakNextTask()
        {
            DB.AddTask(TaskTest1);
            DB.AddTask(TaskTest2);
            Assert.AreEqual(TaskTest1.TaskID, DB.PeakNextTask().TaskID);
        }

        [Test]
        public void PullTask()
        {
            DB.AddTask(TaskTest1);
            DB.AddTask(TaskTest2);
            TaskData pulledTask = DB.PullTask(TaskTest1.TaskID, new NetworkMessage
            {
                SenderIP = "192.168.1.1"
            });
            Assert.AreEqual(true, pulledTask.Active);
            Assert.AreEqual("192.168.1.1", pulledTask.AssignedAddress);
        }

        [Test]
        public void PullTaskNegative()
        {
            DB.AddTask(TaskTest1);
            DB.AddTask(TaskTest2);
            TaskData pulledTask = DB.PullTask(TaskTest1.TaskID, new NetworkMessage
            {
                SenderIP = "192.168.1.1"
            });
            
            try
            {
                TaskData pulledTask2 = DB.PullTask(TaskTest1.TaskID, new NetworkMessage
                {
                    SenderIP = "192.168.1.1"
                });
            }
            catch(Exception)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void PullNextTask()
        {
            DB.AddTask(TaskTest1);
            DB.AddTask(TaskTest2);
            Assert.AreEqual(false, DB.PeakTask(TaskTest1.TaskID).Active);
            TaskData actual = DB.PullNextTask(new NetworkMessage
            {
                SenderIP = "192.168.1.1"
            });
            Assert.AreEqual(TaskTest1.TaskID, actual.TaskID);
        }

        [Test]
        public void PullAllTasks()
        {
            DB.AddTask(TaskTest1);
            DB.AddTask(TaskTest2);
            List<TaskData> result = DB.PullAllTasks(new NetworkMessage
            {
                SenderIP = "192.168.1.1"
            });
            Assert.AreEqual(true, DB.PeakTask(TaskTest1.TaskID).Active);
            Assert.AreEqual(true, DB.PeakTask(TaskTest2.TaskID).Active);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void RequeueTask()
        {
            DB.AddTask(TaskTest1);
            DB.AddTask(TaskTest2);
            TaskData pulledTask = DB.PullTask(TaskTest1.TaskID, new NetworkMessage
            {
                SenderIP = "192.168.1.1"
            });
            Assert.AreEqual(true, pulledTask.Active);
            DB.RequeueTask(pulledTask.TaskID);
            Assert.AreEqual(false, DB.PeakTask(TaskTest1.TaskID).Active);
        }

        [Test]
        public void TaskComplete()
        {
            DB.AddTask(TaskTest1);
            DB.AddTask(TaskTest2);
            TaskData pulledTask = DB.PullTask(TaskTest1.TaskID, new NetworkMessage
            {
                SenderIP = "192.168.1.1"
            });
            DB.TaskComplete(TaskTest1.TaskID, true);
            // TODO: better assert
        }
    }
}
