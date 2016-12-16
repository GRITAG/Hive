using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Core.PackageObjects
{
    /// <summary>
    /// The transmitable package
    /// </summary>
    public class PackageTransmit : PackageData
    {
        public byte[] Data { get; set; }
    }
}
