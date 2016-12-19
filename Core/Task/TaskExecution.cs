using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiveSuite.Core.Task
{
    public class TaskExecution
    {
        /// <summary>
        /// Logger reference object
        /// </summary>
        static Logger Logger { get; set; }
        /// <summary>
        /// Settings reference object
        /// </summary>
        static ISettings Settings { get; set; }
        /// <summary>
        /// The thread to run our task within
        /// </summary>
        static Thread ExecutionThread { get; set; }
        /// <summary>
        /// Task data to be used to execute the task
        /// </summary>
        static TaskData Task { get; set; }
        /// <summary>
        /// Package information
        /// </summary>
        static PackageData Package { get; set; }
        /// <summary>
        /// The start time of task execution 
        /// </summary>
        static DateTime StartTime { get; set; }

        /// <summary>
        /// Is the task running
        /// </summary>
        public bool Running
        {
            get
            {
                return ExecutionThread.IsAlive;
            }
        }
        /// <summary>
        /// the total run time of the task
        /// </summary>
        public TimeSpan RunTime
        {
            get
            {
                return DateTime.Now - StartTime;
            }
        }

        public TaskExecution(TaskData task, PackageData package, Logger logger, ISettings settings)
        {
            Logger = logger;
            Settings = settings;
            Task = task;
            Package = package;
        }

        /// <summary>
        /// Run the task
        /// </summary>
        public void Run()
        {
            ExecutionThread = new Thread(Exeicute);
            ExecutionThread.Start();
        }

        /// <summary>
        /// Stop task execution 
        /// </summary>
        public void Stop()
        {
            if(ExecutionThread.IsAlive)
            {
                ExecutionThread.Abort();
            }

            throw new Exception("Task is not running to stop");
        }

        /// <summary>
        /// The function ran during task execution 
        /// </summary>
        public static void Exeicute()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", "/C " + Task.TaskFile);
            startInfo.WorkingDirectory = Package.Path;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;

            process.Start();
        }
    }
}
