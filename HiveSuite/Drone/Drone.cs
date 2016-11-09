using System;
using System.Threading.Tasks;
using HiveSuite.Core;
using HiveSuite.Core.Network;
using System.Net;

namespace HiveSuite.Drone
{


    /// <summary>
    /// Main loop
    /// </summary>
    public class Drone
    {

        protected static Network ComObject { get; set; }

        protected static DroneState States { get; set; }

        protected static Task CurrentTask { get; private set; }

        protected static DroneSettings Settings { get; set; }

        public static Logger Loging = new Logger();

        public static void MainLoop()
        {
            while (States.CurrentState !=State.ShuttingDown)
            {
                switch (States.CurrentState)
                {
                    case State.StartingUP:
                        try
                        {
                            // Load The settings 
                            if (LoadSettings() && InitilizeComms())
                            {
                                //If we can communicate we can Talk to the TaskMaster or Queen
                                if (ConnectToTaskMaster())
                                {
                                    //If we can talk to the task master We should be ready to do something
                                    States.UpdateState(States.DesiredState);
                                    break;
                                }
                                else
                                {
                                    States.UpdateState(State.ErrorFault);
                                }
                            }
                            else
                            {
                                States.UpdateState(State.ShuttingDown);
                            }
                            
                        }
                        catch (Exception e)
                        {

                            States.UpdateState(State.ShuttingDown);
                            Log("Fail During Startup", e);
                        }

                        break;
                    case State.StartingTask:
                        States.UpdateStatus(Status.NotReadyForWork);
                        //TODO: start processing the tasks or at least doing the set up
                        break;
                    case State.StoppingTask:
                        //TODO: Start Code to tear down any worker threads or at least acknowledge that the thread is done
                        States.UpdateStatus(Status.NotReadyForWork);
                        break;
                    case State.CleaningUP:
                        //TODO: clean up any assets left over from the execution
                        States.UpdateStatus(Status.NotReadyForWork);
                        break;
                    case State.ResettingWorkspace:
                        //TODO: Clean up any working folders 
                        States.UpdateStatus(Status.NotReadyForWork);
                        break;
                    case State.Restarting:
                        //TODO: code for closing coms and flusing
                        States.UpdateStatus(Status.NotReadyForWork);
                        break;
                    case State.ShuttingDown:
                        States.UpdateStatus(Status.NotReadyForWork);
                        break;
                    case State.ErrorFault:
                        //TODO: Need to Log All status and objects before shutting down 
                        States.UpdateStatus(Status.NotReadyForWork);
                        break;
                    case State.Ready:
                        States.UpdateStatus(Status.ReadyForWork);
                        
                        break;
                    default:
                        break;
                }


                switch (States.CurrentStatus)
                {
                    case Status.ReadyForWork:
                        //Connect to TaskMaster and Check for Available Task
                        break;
                    case Status.NotReadyForWork:

                        break;
                    default:
                        break;
                }



            }
            
                       


        }

        private static  void Log(string v, Exception e)
        {
            Loging.Log(LogLevel.Error, v + "\n Exception Information: " + e.ToString());
        }

        private static void Log(string v, LogLevel level = LogLevel.Info)
        {
            Loging.Log(LogLevel.Info, v);
        }

        private static bool ConnectToTaskMaster()
        {
            ComObject.SendMessage(new NetworkMessage
            {
                Message = "Ready"
            }, Settings.ServerIP, Settings.Port);

            NetworkMessage ackMsg;
            DateTime StartTime = DateTime.Now;

            do
            {
                ackMsg = ComObject.PullMessage("Added to Server");

            }
            while (ackMsg == null && (DateTime.Now - StartTime) < new TimeSpan(0,0,60));

            if(ComObject.PeerCount < 0 && ackMsg != null)
            {
                return true;
            }

            return false;
        }

        private static bool InitilizeComms()
        {
            ComObject = new Network(Settings.ServerIP, Settings.Port, Loging);

            return ComObject.CommsUp();
        }

        private static bool LoadSettings()
        {
            Settings = new DroneSettings();
            Settings.Load(Settings.DefaultPath);

            if (string.IsNullOrEmpty(Settings.ServerAddress) || Settings.Port == 0)
            {
                // Gen config and reload
                Log("Could not find a config file for hive under " + Settings.DefaultPath + ". Creating a new default config.");
                DroneSettings.GenerateConfig();
                Settings.Load(Settings.DefaultPath);
                if (string.IsNullOrEmpty(Settings.ServerAddress) || Settings.Port == 0)
                {
                    return false;
                }
            }
            return true;
        }
        // TODO: Communications - Layer
        // TODO: Talk to Hive - task
        // TODO: Init Binary Cache - task / layer
        // TODO: Execute Task - 
        // TODO: Cleanup - task
        // TODO: Close Comm - task


    }
}
