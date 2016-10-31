using HiveSuite.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Drone
{
    public class DroneSettings : ISettings
    {
        public Uri ServerUri { get; set; }
        public int Port { get; set; }

        public string DefaultPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Hive\\Drone\\settings.json";
            }
        }

        public void Load(string filePath)
        {
            FileInfo settingsFile = new FileInfo(filePath);

            if (!settingsFile.Directory.Exists)
            {
                // TODO: Log error - Aaron V.
            }

            if (settingsFile.Exists)
            {
                JsonSerializer serlizer = new JsonSerializer();
                DroneSettings settings = (DroneSettings)JsonConvert.DeserializeObject(filePath, typeof(DroneSettings));

                ServerUri = settings.ServerUri;
                Port = settings.Port;
            }
            else
            {
                // TODO: Log error - Aaron V.
                
            }

        }

        public void Save(string filePath)
        {
            FileInfo settingsFile = new FileInfo(filePath);

            if(!settingsFile.Directory.Exists)
            {
                settingsFile.Directory.Create();
            }

            if (settingsFile.Exists)
            {
                File.Delete(filePath);
            }

            JsonSerializer serlizer = new JsonSerializer();
            using (StreamWriter stream = new StreamWriter(filePath))
            {
                using (JsonWriter writer = new JsonTextWriter(stream))
                {
                    serlizer.Serialize(writer, this);
                }
            }

        }
    }
}
