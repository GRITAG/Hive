using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Core
{
    public class TaskData
    {
        Guid TaskID { get; set; }
        public Guid PackageID { get; set; }
        public byte[] PackageHash { get; set; }
        public string TaskFile { get; set; }
    }
}
