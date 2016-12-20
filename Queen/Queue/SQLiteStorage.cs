using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveSuite.Core;
using HiveSuite.Core.Network;
using System.IO;
using System.Data.SQLite;

namespace HiveSuite.Queen.Queue
{
    public class SQLiteStorage : IQueueStorage
    {
        ISettings Settings { get; set; }
        SQLiteConnection DBConnection { get; set; }


        public SQLiteStorage(ISettings settings)
        {
            Settings = settings;
            if(!File.Exists(settings.DefaultPath + "\\queue.sqlite"))
            {
                SQLiteConnection.CreateFile(settings.DefaultPath + "\\queue.sqlite");
            }

            DBConnection = new SQLiteConnection("Data Source=" + settings.DefaultPath + "\\queue.sqlite" + ";Version=3;");
            DBConnection.Open();

            SQLiteCommand checkForQueueTable = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='Queue';", DBConnection);
            SQLiteCommand checkForDoneTable = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='Done';", DBConnection);

            if (!checkForQueueTable.ExecuteReader(System.Data.CommandBehavior.Default).HasRows && 
                !checkForDoneTable.ExecuteReader(System.Data.CommandBehavior.Default).HasRows)
            {
                CreateTables();
            }

            // TEMP
            AddTask(new TaskData
            {
                TaskID = Guid.NewGuid(),
                PackageID = Guid.NewGuid(),
                PackageHash = new byte[10],
                TaskFile = "test",
                Result = TaskResultType.Failed,
                Active = false,
                AssignedAddress = "1234567890"
            });
        }

        private void CreateTables()
        {
            string createQueue = "CREATE TABLE Queue (TaskID TEXT, PackageID TEXT, PackageHash BLOB, TaskFile TEXT, Result INTEGER, AssignedAddress TEXT, Active BOOLEAN);";
            string createDone = "CREATE TABLE Done (TaskID TEXT, PackageID TEXT, PackageHash BLOB, TaskFile TEXT, Result INTEGER, AssignedAddress TEXT, Active BOOLEAN);";

            SQLiteCommand createQueueCommand = new SQLiteCommand(createQueue, DBConnection);
            SQLiteCommand createDoneCommand = new SQLiteCommand(createDone, DBConnection);
            createQueueCommand.ExecuteNonQuery();
            createDoneCommand.ExecuteNonQuery();
        }

        public void AddTask(TaskData task)
        {
            int activeValue = 0;

            if(task.Active)
            {
                activeValue = 1;
            }

            string command = "INSERT INTO Queue (TaskID, PackageID, PackageHash, TaskFile, Result, AssignedAddress, Active) VALUES (" +
                "'" + task.TaskID.ToString() + "'," +
                "'" + task.PackageID.ToString() + "','" +
                Encoding.ASCII.GetString(task.PackageHash) + "'," +
                "'" + task.TaskFile + "'," +
                (int)task.Result + "," +
                "'" + task.AssignedAddress + "'," +
                activeValue + ");";

            SQLiteCommand addTaskCommand = new SQLiteCommand(command, DBConnection);
            addTaskCommand.ExecuteNonQuery();
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

        public void RemoveTask(Guid id)
        {
            throw new NotImplementedException();
        }

        public void RequeueTask(Guid id)
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
