using System;
using System.IO;
using System.IO.Compression;

namespace HiveSuite.Core
{
    public class Package : PackageData
    {
        public byte[] Binary { get; set; }
        public string ZipFilePath { get; set; }

        public Package(PackageData data)
        {
            if(Directory.Exists(data.Path))
            {
                ZipFilePath = Directory.GetCurrentDirectory() + "data" + DateTime.Now.DayOfYear + DateTime.Now.Year +
                    DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;

                ZipFile.CreateFromDirectory(Path, ZipFilePath);

                MemoryStream stream = new MemoryStream();
                byte[] zipData;
                zipData = File.ReadAllBytes(ZipFilePath);
                stream.Close();
                File.Delete(ZipFilePath);

                Binary = zipData;
            }
        }
    }
}
