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
        protected ISettings Settings { get; set; }

        public Logger Loging = new Logger();

        protected void Log(string v, Exception e)
        {
            Loging.Log(LogLevel.Error, v + "\n Exception Information: " + e.ToString());
        }

        protected void Log(string v, LogLevel level = LogLevel.Info)
        {
            Loging.Log(LogLevel.Info, v);
        }

        public abstract void MainLoop();

        public void GenerateConfig()
        {
            Settings.GenerateConfig();
        }

        protected abstract bool InitilizeComms();

        protected abstract bool LoadSettings();
    }
}
