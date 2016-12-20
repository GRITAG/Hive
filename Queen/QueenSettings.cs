using HiveSuite.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Queen
{
    public class QueenSettings : ISettings
    {
        public int NetworkTimeout { get; set; }
        public int Port { get; set; }

        [JsonIgnore]
        private Core.Logger Logging { get; set; }

        [JsonIgnore]
        public string DefaultFilePath
        {
            get
            {
                return DefaultPath + "\\queensettings.json";
            }
        }

        [JsonIgnore]
        public string DefaultPath
        {
            get
            {
                if (StaticState.unitTesting)
                {
                    return Directory.GetCurrentDirectory();
                }

                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Hive\\Drone";
            }
        }

        public QueenSettings(Core.Logger logger)
        {
            Logging = logger;
        }

        public void GenerateConfig()
        {
            Port = 1000;
            NetworkTimeout = 60;
            Save(DefaultFilePath);
        }

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
                QueenSettings settings = (QueenSettings)JsonConvert.DeserializeObject(File.ReadAllText(filePath), typeof(QueenSettings));

                Port = settings.Port;
                NetworkTimeout = settings.NetworkTimeout;
            }
            else
            {
                Logging.Log(Core.LogLevel.Error, "The Settings file " + filePath + " can not be found");
                throw new FileNotFoundException("Could not find the settings file: " + filePath);

            }
        }

        public void Save(string filePath)
        {
            FileInfo settingsFile = new FileInfo(filePath);

            if (!settingsFile.Directory.Exists)
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
