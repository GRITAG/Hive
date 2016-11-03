using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite
{
    class Program
    {
        static void Main(string[] args)
        {
            switch(args[0].ToLower())
            {
                case "drone":
                    Drone.Drone.Main();
                    break;
            }
        }
    }
}
