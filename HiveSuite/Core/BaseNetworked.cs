using HiveSuite.Drone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Core
{
    public abstract class BaseNetworked
    {
        protected Network.Network ComObject { get; set; }

        protected DroneState States { get; set; }

        protected Task CurrentTask { get; private set; }

        protected DroneSettings Settings { get; set; }

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

        protected abstract bool InitilizeComms();

        protected abstract bool LoadSettings();
    }
}
