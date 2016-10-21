using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace HiveMind
{
    public class CoreLogic
    {
        
        //The IP address of this Unit
        public IPAddress address { get;  private set; }

        //The designation of this unit how it relates in the hive can be used with IP address if multiple units exisit on one machine
        public int PortDesignation { get; private set; }

        // This is the Hive that this node will treate as the central distributer
        public IPAddress HiveCenter { get; set; }
        
        // The Status of the Current Componanat   
        public Status CurrentStatus { get; private set; }

        public CoreLogic()
        {
            address = getIpAddress();

        }


        /// <summary>
        /// Should return and IP Address of the Current System 
        /// </summary>
        /// <returns></returns>
        private IPAddress getIpAddress()
        {

            
         var host= Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip;
                }
            else
                {
                    continue;
                }

            throw new Exception("NoIpAddressFound");
           //TODO:  Add methods to return the localsystmes IP Address for broadcast
        }

    }

    public enum Status
    {
        StartingUP,
        StartingTask,
        StoppingTask,
        CleaningUP,
        ResettingWorkspace,
        Restarting,
        ShuttingDown,
        ErrorFault,
        ReadyForWork,
        NotReadyForWork       
        
    }
}
