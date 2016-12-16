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
        public Guid TaskID { get; set; }
        public Guid PackageID { get; set; }
        public byte[] PackageHash { get; set; }
        public string TaskFile { get; set; }

        public TaskData() { }

        public TaskData(string json)
        {
            TaskData testTemp = JsonConvert.DeserializeObject<TaskData>(json);

            TaskID = testTemp.TaskID;
            PackageID = testTemp.PackageID;
            PackageHash = testTemp.PackageHash;
            TaskFile = testTemp.TaskFile;
        }
    }
}
