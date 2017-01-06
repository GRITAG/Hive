using HiveSuite.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HiveSuite.Queen.PackageObjects
{
    public class PackageHandler
    {
        PackageCache Packages { get; set; }
        // TODO: lock for threading

        public PackageHandler(ISettings settings)
        {
            Packages = new PackageCache(settings);
        }

        public Package GetPackage(Guid id, byte[] mD5Hash)
        {
            Package result = new Package(Packages.GetPackage(id, mD5Hash));
            return result;
        }
    }
}
