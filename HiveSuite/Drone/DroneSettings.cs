using HiveSuite.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Drone
{
    public class DroneSettings : ISettings
    {
        public string ServerAddress { get; set; }
        public int Port { get; set; }
        [JsonIgnore]
        public IPAddress ServerIP
        {
            get
            {
                return IPAddress.Parse(ServerAddress);
            }
        }
        [JsonIgnore]
        public string DefaultPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Hive\\Drone\\settings.json";
            }
        }

        /// <summary>
        /// Creates a drone settings object with default values
        /// </summary>
        public DroneSettings()
        {
            ServerAddress = string.Empty;
            Port = 0;
        }

        /// <summary>
        /// Loads a drone settings file
        /// </summary>
        /// <param name="filePath">full path to file</param>
        public void Load(string filePath)
        {
            FileInfo settingsFile = new FileInfo(filePath);

            if (!settingsFile.Directory.Exists)
            {
                Drone.Loging.Log(LogLevel.Error, "The Settings dir " + settingsFile.Directory + " can not be found");
            }

            if (settingsFile.Exists)
            {
                JsonSerializer serlizer = new JsonSerializer();
                DroneSettings settings = (DroneSettings)JsonConvert.DeserializeObject(File.ReadAllText(filePath), typeof(DroneSettings));

                ServerAddress = settings.ServerAddress;
                Port = settings.Port;
                ServerAddress = settings.ServerAddress;
            }
            else
            {
                Drone.Loging.Log(LogLevel.Error, "The Settings file " + filePath + " can not be found");

            }

        }

        /// <summary>
        /// Saves a drone settings file
        /// </summary>
        /// <param name="filePath">full path to save file</param>
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

        /// <summary>
        /// Creates a default config and saves it to the default path
        /// </summary>
        public static void GenerateConfig()
        {
            DroneSettings Settings = new DroneSettings();
            Settings.Port = 1000;
            Settings.ServerAddress = "192.168.1.100";

            Settings.Save(Settings.DefaultPath);
        }
    }
}
