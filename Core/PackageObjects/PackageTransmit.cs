using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        public PackageTransmit() { }

        public PackageTransmit(string json)
        {
            PackageTransmit package = JsonConvert.DeserializeObject<PackageTransmit>(json);

            Data = package.Data;
            ID = package.ID;
            MD5Hash = package.MD5Hash;
            Path = package.Path;
        }

        public byte[] GetMd5Hash()
        {
            byte[] rawHash;
            using (MD5 hashCalc = MD5.Create())
            {
                rawHash = hashCalc.ComputeHash(Data);
            }
            
            StringBuilder result = new StringBuilder();

            for (int byteIndex = 0; byteIndex < rawHash.Length; byteIndex++)
            {
                result.Append(rawHash[byteIndex].ToString("x2"));
            }

            return rawHash;
        }
    }
}
