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
        public static BaseNetworked Application { get; set; }

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
                    Application.GenerateConfig();
                    break;
            }

            Application.MainLoop();
        }
    }
}
