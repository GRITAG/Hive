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
        Guid PackageID { get; set; }
        byte[] PackageHash { get; set; }
        string TaskFile { get; set; }
    }
}
