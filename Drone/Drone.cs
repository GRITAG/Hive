using System;
using System.Threading.Tasks;
using HiveSuite.Core;
using HiveSuite.Core.Network;
using System.Net;
using HiveSuite.Core.PackageObjects;
using HiveSuite.Core.Task;
using System.Threading;

namespace HiveSuite.Drone
{


    /// <summary>
    /// Main loop
    /// </summary>
    public class Drone : BaseNetworked
    {
        NetworkClient ComObject { get; set; }

        /// <summary>
        /// Drone state object
        /// </summary>
        protected DroneState States { get; set; }

        protected TaskExecution TaskExe { get; set; }

        protected TaskData CurrentTaskData { get; set; }
        
        protected PackageCache Cache { get; set; }

        public Drone() : base()
        {
            States = new DroneState();
            this.Loging = new Logger();
        }

        /// <summary>
        /// loop for the objet type
        /// </summary>
        public override void MainLoop()
        {
            NetworkMessage incoming = null;

            while (States.CurrentState !=State.ShuttingDown)
            {
                #region StateManagment
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
                    case State.CheckForPackage:
                        if(!Cache.ContainsPackage(CurrentTaskData.PackageID, CurrentTaskData.PackageHash))
                        {
                            ComObject.SendMessage(NetworkMessages.RequestPackage(CurrentTaskData.PackageID, CurrentTaskData.PackageHash));
                            States.UpdateState(State.WaitingForPackage);
                        }
                        break;
                    case State.StartingTask:
                        States.UpdateStatus(Status.NotReadyForWork);
                        TaskExe = new TaskExecution(CurrentTaskData, Cache.GetPackage(CurrentTaskData.PackageID, CurrentTaskData.PackageHash), Loging, Settings);
                        States.UpdateState(State.Running);
                        break;
                    case State.Running:
                        States.UpdateStatus(Status.NotReadyForWork);
                        if(TaskExe.Running)
                        {
                            if (TaskExe.RunTime > new TimeSpan(0, ((DroneSettings)Settings).ExecutionTimeout, 0))
                            {
                                States.UpdateState(State.StoppingTask);
                            }
                        }
                        else
                        {
                            States.UpdateState(State.CleaningUP);
                        }
                        break;
                    case State.StoppingTask:
                        TaskExe.Stop();
                        States.UpdateStatus(Status.NotReadyForWork);
                        break;
                    case State.CleaningUP:
                        States.UpdateState(State.Ready);
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
                        ComObject.SendMessage(NetworkMessages.RequestTask);
                        States.UpdateState(State.WaitingForTask);
                        break;
                    case State.WaitingForTask:
                        LastInteraction = DateTime.Now;
                        while (incoming == null && (LastInteraction - DateTime.Now) < new TimeSpan(0, 2, 0))
                        {
                            incoming = ComObject.PullMessage(NetworkMessages.ResponseTask(null).Message);
                            Thread.Sleep(500);
                        }

                        if(incoming == null)
                        {
                            Log("Task retirval timed out", LogLevel.Error);
                            States.UpdateState(State.Ready);
                        }
                        else
                        {
                            CurrentTaskData = new TaskData(incoming.Data.ToString());
                            incoming = null;
                            States.UpdateState(State.CheckForPackage);
                        }

                        break;
                    case State.WaitingForPackage:
                        incoming = ComObject.PullMessage(NetworkMessages.ResponsePackage(null).Message);
                        if (incoming != null)
                        {
                            Cache.AddPackages(new PackageTransmit(incoming.Data.ToString()));
                        }
                        incoming = null;
                        break;
                    default:
                        break;
                }
                #endregion

                #region StatusManagment
                //switch (States.CurrentStatus)
                //{
                //    case Status.ReadyForWork:
                        
                //        States.UpdateStatus(Status.WaitingForWork);
                //        break;
                //    case Status.WaitingForWork:

                //        break;
                //    case Status.NotReadyForWork:

                //        break;
                //    default:
                //        break;
                //}
                #endregion

                #region NetMessageManagment
                // get all network messages
                // check for state needed messages first
                // react to other messages
                //switch(States.CurrentState)
                //{
                //    case State.Ready:
                //        switch(States.CurrentStatus)
                //        {
                //            case Status.ReadyForWork:
                                
                //                break;
                //        }
                //        break;
                //}
                //ComObject.PullMessage("");
                #endregion

            }
        }

        /// <summary>
        /// Set up all external communication
        /// </summary>
        /// <returns></returns>
        protected override bool InitilizeComms()
        {
            ComObject = new NetworkClient(Settings);
            ComObject.SendDiscovery();

            return ComObject.CommsUp();
        }

        /// <summary>
        /// Load the settings file for the object type
        /// </summary>
        /// <returns></returns>
        protected override bool LoadSettings()
        {
            Settings = new DroneSettings(Loging);
            Settings.Load(Settings.DefaultFilePath);

            if (string.IsNullOrEmpty(((DroneSettings)Settings).ServerAddress) || ((DroneSettings)Settings).Port == 0)
            {
                // Gen config and reload
                Log("Could not find a config file for hive under " + Settings.DefaultPath + ". Creating a new default config.");
                Settings = new DroneSettings(Loging);
                Settings.GenerateConfig();
                Settings.Load(Settings.DefaultPath);
                if (string.IsNullOrEmpty(((DroneSettings)Settings).ServerAddress) || ((DroneSettings)Settings).Port == 0)
                {
                    return false;
                }
            }

            Cache = new PackageCache(Settings);

            return true;
        }

        /// <summary>
        /// Startes and resolves the handshaking with the server
        /// </summary>
        /// <returns></returns>
        private bool ConnectToTaskMaster()
        {
            ComObject.SendMessage(NetworkMessages.Ready);

            NetworkMessage ackMsg;
            DateTime StartTime = DateTime.Now;

            do
            {
                ackMsg = ComObject.PullMessage(NetworkMessages.AddToServer.Message);

            }
            while (ackMsg == null && (DateTime.Now - StartTime) < new TimeSpan(0, 0, 60));

            if (ComObject.PeerCount > 0 && ackMsg != null)
            {
                return true;
            }

            return false;
        }

        // TODO: Communications - Layer
        // TODO: Talk to Hive - task
        // TODO: Init Binary Cache - task / layer
        // TODO: Execute Task - 
        // TODO: Cleanup - task
        // TODO: Close Comm - task


    }
}
