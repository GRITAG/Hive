﻿using System;
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
        }

        /// <summary>
        /// Creates a table set for a new queue store
        /// </summary>
        private void CreateTables()
        {
            string createQueue = "CREATE TABLE Queue (TaskID TEXT, PackageID TEXT, PackageHash BLOB, TaskFile TEXT, Result INTEGER, AssignedAddress TEXT, Active BOOLEAN);";
            string createDone = "CREATE TABLE Done (TaskID TEXT, PackageID TEXT, PackageHash BLOB, TaskFile TEXT, Result INTEGER, AssignedAddress TEXT, Active BOOLEAN);";

            SQLiteCommand createQueueCommand = new SQLiteCommand(createQueue, DBConnection);
            SQLiteCommand createDoneCommand = new SQLiteCommand(createDone, DBConnection);
            createQueueCommand.ExecuteNonQuery();
            createDoneCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// helper method to read a byte array / blob from a result
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private static byte[] GetBytes(SQLiteDataReader reader, int col)
        {
            const int CHUNK_SIZE = 2 * 1024;
            byte[] buffer = new byte[CHUNK_SIZE];
            long bytesRead;
            long fieldOffset = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                while ((bytesRead = reader.GetBytes(col, fieldOffset, buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, (int)bytesRead);
                    fieldOffset += bytesRead;
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Used to read a single taskdata result
        /// </summary>
        /// <param name="resultReader"></param>
        /// <returns></returns>
        private TaskData ReadTaskData(SQLiteDataReader resultReader)
        {
            TaskData result = null;
            resultReader.Read();

            if (resultReader.HasRows)
            {
                result = new TaskData();

                result.TaskID = Guid.Parse(resultReader.GetString(0));
                result.PackageID = Guid.Parse(resultReader.GetString(1));
                result.PackageHash = GetBytes(resultReader, 2);
                result.TaskFile = resultReader.GetString(3);
                switch (resultReader.GetInt32(4))
                {
                    case 0:
                        result.Result = TaskResultType.None;
                        break;
                    case 1:
                        result.Result = TaskResultType.Passed;
                        break;
                    case 2:
                        result.Result = TaskResultType.Failed;
                        break;
                }
                result.AssignedAddress = resultReader.GetString(5);
                result.Active = resultReader.GetBoolean(6);
            }
            return result;
        }

        /// <summary>
        /// Read all task data in the queue
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private List<TaskData> ReadTasksData(SQLiteDataReader reader)
        {
            List<TaskData> tasks = new List<TaskData>();
            TaskData currentTask = null;

            while (reader.Read())
            {
                currentTask = new TaskData();

                currentTask.TaskID = Guid.Parse(reader.GetString(0));
                currentTask.PackageID = Guid.Parse(reader.GetString(1));
                currentTask.PackageHash = GetBytes(reader, 2);
                currentTask.TaskFile = reader.GetString(3);
                switch (reader.GetInt32(4))
                {
                    case 0:
                        currentTask.Result = TaskResultType.None;
                        break;
                    case 1:
                        currentTask.Result = TaskResultType.Passed;
                        break;
                    case 2:
                        currentTask.Result = TaskResultType.Failed;
                        break;
                }
                currentTask.AssignedAddress = reader.GetString(5);
                currentTask.Active = reader.GetBoolean(6);
                tasks.Add(currentTask);
            }

            return tasks;
        }

        /// <summary>
        /// Add a task to the queue
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(TaskData task)
        {
            int activeValue = 0;

            if(task.Active)
            {
                activeValue = 1;
            }

            string command = "INSERT INTO Queue (TaskID, PackageID, PackageHash, TaskFile, Result, AssignedAddress, Active) VALUES (" +
                "'" + task.TaskID.ToString() + "'," +
                "'" + task.PackageID.ToString() + "'," +
                "@hash," +
                "'" + task.TaskFile + "'," +
                (int)task.Result + "," +
                "'" + task.AssignedAddress + "'," +
                activeValue + ");";

            SQLiteCommand addTaskCommand = new SQLiteCommand(command, DBConnection);
            addTaskCommand.Parameters.Add("@hash", System.Data.DbType.Binary).Value = task.PackageHash;
            addTaskCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Peak at all tasks in the queue
        /// </summary>
        /// <returns></returns>
        public List<TaskData> PeakAllTasks()
        {
            string command = "SELECT * FROM queue;";
            SQLiteCommand peakAllCommand = new SQLiteCommand(command, DBConnection);
            SQLiteDataReader reader = peakAllCommand.ExecuteReader();
            return ReadTasksData(reader);
        }

        /// <summary>
        /// peak at the net task
        /// </summary>
        /// <returns></returns>
        public TaskData PeakNextTask()
        {
            string command = "SELECT * FROM queue;";
            SQLiteCommand peakNext = new SQLiteCommand(command, DBConnection);
            SQLiteDataReader reader = peakNext.ExecuteReader(System.Data.CommandBehavior.SingleResult);
            return ReadTaskData(reader);
        }

        /// <summary>
        /// Peak at a task in the queue
        /// </summary>
        /// <param name="id">the id of the task to peak at</param>
        /// <returns></returns>
        public TaskData PeakTask(Guid id)
        {
            string command = "SELECT * FROM queue WHERE TaskID='" + id.ToString() + "';";
            SQLiteCommand peakCommand = new SQLiteCommand(command, DBConnection);
            SQLiteDataReader resultReader = peakCommand.ExecuteReader();
            return ReadTaskData(resultReader);
        }

        /// <summary>
        /// Pull all none active tasks from the queue
        /// </summary>
        /// <param name="revicer"></param>
        /// <returns></returns>
        public List<TaskData> PullAllTasks(NetworkMessage revicer)
        {
            string getAllTasks = "SELECT * FROM queue WHERE Active = 0;";
            string update;
            SQLiteCommand updateCommand;
            SQLiteCommand getAllTasksCommand = new SQLiteCommand(getAllTasks, DBConnection);
            List<TaskData> result = ReadTasksData(getAllTasksCommand.ExecuteReader());

            if(result.Count > 0)
            {
                foreach(TaskData currentTask in result)
                {
                    update = "UPDATE queue SET active = 1, AssignedAddress = '" + revicer.SenderIP + "' WHERE TaskID='" + currentTask.TaskID.ToString() + "';";
                    updateCommand = new SQLiteCommand(update, DBConnection);
                    updateCommand.ExecuteNonQuery();
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// Pull the next task to be run
        /// </summary>
        /// <param name="revicer"></param>
        /// <returns></returns>
        public TaskData PullNextTask(NetworkMessage revicer)
        {
            string findTask = "SELECT * FROM queue WHERE Active = 0;";
            
            SQLiteCommand findTaskCommand = new SQLiteCommand(findTask, DBConnection);
            SQLiteDataReader reader = findTaskCommand.ExecuteReader(System.Data.CommandBehavior.SingleResult);
            TaskData foundTask = ReadTaskData(reader);

            if (foundTask != null)
            {
                string update = "UPDATE queue SET active = 1, AssignedAddress = '" + revicer.SenderIP + "' WHERE TaskID='" + foundTask.TaskID.ToString() + "';";
                SQLiteCommand updateCommand = new SQLiteCommand(update, DBConnection);
                updateCommand.ExecuteNonQuery();
                return foundTask;
            }
            else
            {
                throw new Exception("No tasks in queue");
            }
        }

        /// <summary>
        /// Pull th task with the given id and assign execution to the network message sender passed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="revicer"></param>
        /// <returns></returns>
        public TaskData PullTask(Guid id, NetworkMessage revicer)
        {
            TaskData currentTask = null;

            string select = "SELECT * FROM queue WHERE TaskID='" + id.ToString() + "';";
            string update = "UPDATE queue SET active = 1, AssignedAddress = '" + revicer.SenderIP + "' WHERE TaskID='" + id.ToString() + "';";

            SQLiteCommand selectCommand = new SQLiteCommand(select, DBConnection);
            SQLiteCommand selectCommand2 = new SQLiteCommand(select, DBConnection);
            SQLiteCommand updateCommand = new SQLiteCommand(update, DBConnection);

            if (!ReadTaskData(selectCommand.ExecuteReader()).Active)
            {
                if (updateCommand.ExecuteNonQuery() > 0)
                {
                    currentTask = ReadTaskData(selectCommand2.ExecuteReader());
                }
                else
                {
                    throw new Exception("Could not find task " + id.ToString() + " to update");
                }
            }
            else
            {
                throw new Exception("The task " + id.ToString() + "has already been pulled");
            }

            return currentTask;
        }

        /// <summary>
        /// Remove a task from the Queue
        /// </summary>
        /// <param name="id"></param>
        public void RemoveTask(Guid id)
        {
            TaskData result = PeakTask(id);
            if (result != null)
            {
                string command = "DELETE from queue WHERE TaskID='" + id.ToString() + "';";
                SQLiteCommand peakCommand = new SQLiteCommand(command, DBConnection);
                peakCommand.ExecuteNonQuery();


            }
            else
            {
                throw new Exception("No task by " + id.ToString() + " to remove");
            }
        }

        /// <summary>
        /// Requeue the given task
        /// </summary>
        /// <param name="id"></param>
        public void RequeueTask(Guid id)
        {
            string command = "SELECT * FROM queue WHERE TaskID='" + id.ToString() + "';";
            SQLiteCommand peakCommand = new SQLiteCommand(command, DBConnection);
            SQLiteDataReader resultReader = peakCommand.ExecuteReader();
            TaskData result = ReadTaskData(resultReader);

            if (result != null)
            {
                if (result.Active)
                {
                    string update = "UPDATE queue SET active = 0 WHERE TaskID='" + id.ToString() + "';";
                    SQLiteCommand updateCommand = new SQLiteCommand(update, DBConnection);
                    updateCommand.ExecuteNonQuery();
                }
                else
                {
                    throw new Exception("Task " + id.ToString() + " is already active");
                }
            }
            else
            {
                throw new Exception("Task " + id.ToString() + " could not be found");
            }
        }

        /// <summary>
        /// Retrieve the count of tasks in the queue
        /// </summary>
        /// <returns></returns>
        public int TaskCount()
        {
            string command = "SELECT * FROM queue;";
            SQLiteCommand peakAllCommand = new SQLiteCommand(command, DBConnection);
            SQLiteDataReader reader = peakAllCommand.ExecuteReader();
            return ReadTasksData(reader).Count;
        }

        /// <summary>
        /// Move a task to the complete status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="passFail"></param>
        public void TaskComplete(Guid id, bool passFail)
        {
            string command = "SELECT * FROM queue WHERE TaskID='" + id.ToString() + "';";
            SQLiteCommand peakCommand = new SQLiteCommand(command, DBConnection);
            SQLiteDataReader resultReader = peakCommand.ExecuteReader();
            TaskData result = ReadTaskData(resultReader);

            if (result != null)
            {
                if(passFail)
                {
                    result.Result = TaskResultType.Passed;
                }
                else
                {
                    result.Result = TaskResultType.Failed;
                }

                #region AddToArchive
                int activeValue = 0;

                if (result.Active)
                {
                    activeValue = 1;
                }

                string commandAddArchive = "INSERT INTO Done (TaskID, PackageID, PackageHash, TaskFile, Result, AssignedAddress, Active) VALUES (" +
                    "'" + result.TaskID.ToString() + "'," +
                    "'" + result.PackageID.ToString() + "'," +
                    "@hash," +
                    "'" + result.TaskFile + "'," +
                    (int)result.Result + "," +
                    "'" + result.AssignedAddress + "'," +
                    activeValue + ");";

                SQLiteCommand addTaskCommand = new SQLiteCommand(commandAddArchive, DBConnection);
                addTaskCommand.Parameters.Add("@hash", System.Data.DbType.Binary).Value = result.PackageHash;
                addTaskCommand.ExecuteNonQuery();
                #endregion

                string remove = "DELETE from queue WHERE TaskID='" + result.TaskID.ToString() + "';";
                SQLiteCommand removeCommand = new SQLiteCommand(remove, DBConnection);
                removeCommand.ExecuteNonQuery();
            }
            else
            {
                throw new Exception("Task " + id.ToString() + " could not be found");
            }
        }

        public void Close()
        {
            DBConnection.Close();
        }
    }
}
