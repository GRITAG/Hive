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
        static Logger Logger { get; set; }
        static ISettings Settings { get; set; }
        static Thread ExecutionThread { get; set; }
        static TaskData Task { get; set; }
        static PackageData Package { get; set; }

        public TaskExecution(TaskData task, PackageData package, Logger logger, ISettings settings)
        {
            Logger = logger;
            Settings = settings;
            Task = task;
            Package = package;
        }

        public void Run()
        {
            ExecutionThread = new Thread(Exeicute);
            ExecutionThread.Start();
        }

        public void Stop()
        {
            if(ExecutionThread.IsAlive)
            {
                ExecutionThread.Abort();
            }

            throw new Exception("Task is not running to stop");
        }

        public static void Exeicute()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = Package.Path;
            startInfo.FileName = "cmd.exe " + Task.TaskFile;
            process.StartInfo = startInfo;

            process.Start();
        }
    }
}
