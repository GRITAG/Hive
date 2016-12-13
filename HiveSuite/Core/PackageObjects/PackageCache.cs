using HiveSuite.Core.PackageObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Core
{
    /// <summary>
    /// contior to store package information
    /// </summary>
    public class PackageCache
    {
        List<PackageData> Packages { get; set; }
        ISettings Settings { get; set; }

        public PackageCache(ISettings settings)
        {
            Packages = new List<PackageData>();
            Settings = settings;
        }

        public void AddPackages(PackageTransmit packageToAdd)
        {
            if(Directory.Exists(Settings.DefaultPath + "\\temp"))
            {
                Directory.Delete(Settings.DefaultPath + "\\temp", true);
            }
            else
            {
                Directory.CreateDirectory(Settings.DefaultPath + "\\temp");
            }

            // Write the file sent to a temp dir
            FileStream fileS = new FileStream(Settings.DefaultPath + "\\temp\\" + packageToAdd.ID + ".zip", FileMode.Create);
            fileS.Write(packageToAdd.Data, 0, packageToAdd.Data.Length);

            string packageDir = DateTime.Now.ToString("MMddyy_HHmmss");

            Directory.CreateDirectory(Settings.DefaultPath + "\\packages\\" + packageDir);

            ZipFile.ExtractToDirectory(Settings.DefaultPath + "\\temp\\" + packageToAdd.ID + ".zip", Settings.DefaultPath + "\\packages\\" + packageDir);

            packageToAdd.Path = Settings.DefaultPath + "\\packages\\" + packageDir;

            Packages.Add(packageToAdd);
        }

        public bool ContainsPackage(Guid id, byte[] md5Hash)
        {
            foreach(PackageData currentPack in Packages)
            {
                if(currentPack.ID == id)
                {
                    if (currentPack.MD5Hash.SequenceEqual(md5Hash))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool ContainsPackage(PackageData package)
        {
            return ContainsPackage(package.ID, package.MD5Hash);
        }

        public PackageData GetPackage(Guid id, byte[] md5Hash)
        {
            foreach (PackageData currentPack in Packages)
            {
                if (currentPack.ID == id)
                {
                    if (currentPack.MD5Hash.SequenceEqual(md5Hash))
                    {
                        return currentPack;
                    }
                }
            }

            throw new Exception("Could not find requested pacakge");
        }

        public void RemovePackage(Guid id, byte[] md5Hash)
        {
            for (int packageIndex=0; packageIndex < Packages.Count; packageIndex++) //PackageData currentPack in Packages)
            {
                if (Packages[packageIndex].ID == id)
                {
                    if (Packages[packageIndex].MD5Hash.SequenceEqual(md5Hash))
                    {
                        Packages.RemoveAt(packageIndex);
                    }
                }
            }

            throw new Exception("Package could not be found to be removed");
        }

        public void RemovePackage(PackageData package)
        {
            RemovePackage(package.ID, package.MD5Hash);
        }
    }
}
