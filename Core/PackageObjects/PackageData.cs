using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Core
{
    /// <summary>
    /// Stripped down data only version of the package
    /// </summary>
    public class PackageData
    {
        public Guid ID { get; set; }
        public byte[] MD5Hash { get; set; }
        public string Path { get; set; }

        // TODO: add a package info file thing so we do not have to redownload packages
    }
}
