using HiveSuite.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite
{
    class Program
    {
        /// <summary>
        /// The Application object
        /// </summary>
        public static BaseNetworked Application { get; set; }

        /// <summary>
        /// The outer most main loop
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            args = new string[] { "drone" };

            switch(args[0].ToLower())
            {
                case "drone":
                    Application = new Drone.Drone();
                    break;
                case "configdrone":
                    Application = new Drone.Drone();
                    Application.Settings = new Drone.DroneSettings(Application.Loging);
                    Application.GenerateConfig();
                    Environment.Exit(0);
                    break;
            }

            Application.MainLoop();
            Environment.Exit(0);
        }
    }
}
