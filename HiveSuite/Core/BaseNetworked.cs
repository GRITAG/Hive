using HiveSuite.Drone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Core
{
    /// <summary>
    /// Object is used to house base and shared functions between the drone and queen main objects
    /// </summary>
    public abstract class BaseNetworked
    {
        /// <summary>
        /// Main network object
        /// </summary>
        protected Network.Network ComObject { get; set; }
        
        /// <summary>
        /// Setting componet used to load, save, and read settings
        /// </summary>
        public ISettings Settings { get; set; }

        /// <summary>
        /// Main Logging object
        /// </summary>
        public Logger Loging = new Logger();

        /// <summary>
        /// Create a log entry
        /// </summary>
        /// <param name="v">message for the log</param>
        /// <param name="e">exepction to log</param>
        protected void Log(string v, Exception e)
        {
            Loging.Log(LogLevel.Error, v + "\n Exception Information: " + e.ToString());
        }

        /// <summary>
        /// Create a log entry
        /// </summary>
        /// <param name="v">message</param>
        /// <param name="level">the log level to use</param>
        protected void Log(string v, LogLevel level = LogLevel.Info)
        {
            Loging.Log(LogLevel.Info, v);
        }

        /// <summary>
        /// Main loop to be over loaded
        /// </summary>
        public abstract void MainLoop();

        /// <summary>
        /// Generates a config file to be stored in the default location
        /// </summary>
        public void GenerateConfig()
        {
            Settings.GenerateConfig();
        }

        /// <summary>
        /// Setup comms (Networking / external communication)
        /// </summary>
        /// <returns></returns>
        protected abstract bool InitilizeComms();

        /// <summary>
        /// Load settings for the objects
        /// </summary>
        /// <returns></returns>
        protected abstract bool LoadSettings();
    }
}
