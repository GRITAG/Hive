using HiveSuite.Core;
using Newtonsoft.Json;
using NLog;
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
        /// <summary>
        /// Server address to connect to 
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        /// port to use for communication with the server
        /// </summary>
        public int Port { get; set; }

        public int NetworkTimeout { get; set; }

        public int ExecutionTimeout { get; set; }

        // Next few objects are not stored in the Json config
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
                if(StaticState.unitTesting)
                {
                    return Directory.GetCurrentDirectory() + "\\settings.json";
                }

                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Hive\\Drone";
            }
        }

        [JsonIgnore]
        public string DefaultFilePath
        {
            get
            {
                return DefaultPath + "\\settings.json";
            }
        }

        [JsonIgnore]
        private Core.Logger Logging { get; set; }

        /// <summary>
        /// Creates a drone settings object with default values
        /// </summary>
        public DroneSettings(Core.Logger logger)
        {
            ServerAddress = string.Empty;
            Port = 0;
            Logging = logger;
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
                Logging.Log(Core.LogLevel.Error, "The Settings dir " + settingsFile.Directory + " can not be found");
            }

            if (settingsFile.Exists)
            {
                JsonSerializer serlizer = new JsonSerializer();
                DroneSettings settings = (DroneSettings)JsonConvert.DeserializeObject(File.ReadAllText(filePath), typeof(DroneSettings));

                ServerAddress = settings.ServerAddress;
                Port = settings.Port;
                ServerAddress = settings.ServerAddress;
                NetworkTimeout = settings.NetworkTimeout;
            }
            else
            {
                Logging.Log(Core.LogLevel.Error, "The Settings file " + filePath + " can not be found");
                throw new FileNotFoundException("Could not find the settings file: " + filePath);

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
        public void GenerateConfig()
        {
            Port = 1000;
            ServerAddress = "192.168.1.100";

            Save(DefaultFilePath);
        }

    }
}
