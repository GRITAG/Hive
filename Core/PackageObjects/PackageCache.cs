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
    /// container to store package information
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

        /// <summary>
        /// Adds a package to the Cache
        /// </summary>
        /// <param name="packageToAdd"></param>
        public void AddPackages(PackageTransmit packageToAdd)
        {
            if(Directory.Exists(Settings.DefaultPath + "\\temp"))
            {
                Directory.Delete(Settings.DefaultPath + "\\temp", true);
            }
            
            Directory.CreateDirectory(Settings.DefaultPath + "\\temp");
            

            // Write the file sent to a temp dir
            using (FileStream fileS = new FileStream(Settings.DefaultPath + "\\temp\\" + packageToAdd.ID + ".zip", FileMode.Create))
            {
                fileS.Write(packageToAdd.Data, 0, packageToAdd.Data.Length);
                fileS.Flush();
            }

            string packageDir = DateTime.Now.ToString("MMddyy_HHmmss");

            Directory.CreateDirectory(Settings.DefaultPath + "\\packages\\" + packageDir);

            ZipFile.ExtractToDirectory(Settings.DefaultPath + "\\temp\\" + packageToAdd.ID + ".zip", Settings.DefaultPath + "\\packages\\" + packageDir);

            Directory.Delete(Settings.DefaultPath + "\\temp", true);

            packageToAdd.Path = Settings.DefaultPath + "\\packages\\" + packageDir;


            Packages.Add(packageToAdd);
        }

        /// <summary>
        /// Checks if a package is contained with in the cache
        /// </summary>
        /// <param name="id"></param>
        /// <param name="md5Hash"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Check if a package in contained with in the cache
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public bool ContainsPackage(PackageData package)
        {
            return ContainsPackage(package.ID, package.MD5Hash);
        }

        /// <summary>
        /// Return a package with the given ID and Hash
        /// </summary>
        /// <param name="id"></param>
        /// <param name="md5Hash"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Remove a package via ID and Hash
        /// </summary>
        /// <param name="id"></param>
        /// <param name="md5Hash"></param>
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

        /// <summary>
        /// Remove a package via package object
        /// </summary>
        /// <param name="package"></param>
        public void RemovePackage(PackageData package)
        {
            RemovePackage(package.ID, package.MD5Hash);
        }
    }
}
