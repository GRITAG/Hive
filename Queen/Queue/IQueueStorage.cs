using HiveSuite.Core;
using HiveSuite.Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Queen.Queue
{
    public interface IQueueStorage
    {
        TaskData PullNextTask(NetworkMessage revicer);
        TaskData PullTask(Guid id, NetworkMessage revicer);
        void AddTask(TaskData task);
        List<TaskData> PullAllTasks(NetworkMessage revicer);
        int TaskCount();
        TaskData PeakTask(Guid id);
        TaskData PeakNextTask();
        List<TaskData> PeakAllTasks();
        void TaskComplete(Guid id, bool passFail);
        void RemoveTask(Guid id);
        void RequeueTask(Guid id);
    }
}
