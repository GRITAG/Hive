using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveSuite.Core;
using HiveSuite.Core.Network;

namespace HiveSuite.Queen.Queue
{
    public class SQLiteStorage : IQueueStorage
    {
        public void AddTask(TaskData task)
        {
            throw new NotImplementedException();
        }

        public List<TaskData> PeakAllTasks()
        {
            throw new NotImplementedException();
        }

        public TaskData PeakNextTask()
        {
            throw new NotImplementedException();
        }

        public TaskData PeakTask()
        {
            throw new NotImplementedException();
        }

        public List<TaskData> PullAllTasks(NetworkMessage revicer)
        {
            throw new NotImplementedException();
        }

        public TaskData PullNextTask(NetworkMessage revicer)
        {
            throw new NotImplementedException();
        }

        public TaskData PullTask(Guid id, NetworkMessage revicer)
        {
            throw new NotImplementedException();
        }

        public int TaksCount()
        {
            throw new NotImplementedException();
        }

        public void TaskComplete(Guid id, bool passFail)
        {
            throw new NotImplementedException();
        }
    }
}
