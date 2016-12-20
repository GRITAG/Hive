using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Core
{
    public class TaskData
    {
        /// <summary>
        /// Unique id of a package for tracking
        /// </summary>
        public Guid TaskID { get; set; }
        /// <summary>
        /// The ID of the package need for the task
        /// </summary>
        public Guid PackageID { get; set; }
        /// <summary>
        /// The MD5 hash of the package needed for the task
        /// </summary>
        public byte[] PackageHash { get; set; }
        /// <summary>
        /// The file with in the package to run
        /// </summary>
        public string TaskFile { get; set; }
        public TaskResultType Result { get; set; }

        [JsonIgnore]
        public string AssignedAddress { get; set; }
        [JsonIgnore]
        public bool Active { get; set; }

        public TaskData()
        {
            TaskID = Guid.NewGuid();
            Result = TaskResultType.None;
        }

        /// <summary>
        /// Construct a taskdata object from a Json serialized task data
        /// </summary>
        /// <param name="json"></param>
        public TaskData(string json)
        {
            TaskData testTemp = JsonConvert.DeserializeObject<TaskData>(json);

            TaskID = testTemp.TaskID;
            PackageID = testTemp.PackageID;
            PackageHash = testTemp.PackageHash;
            TaskFile = testTemp.TaskFile;
            Result = testTemp.Result;
        }
    }

    public enum TaskResultType
    {
        None, Passed, Failed
    }
}
