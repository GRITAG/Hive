using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Drone
{
    public class DroneState
    {
        /// <summary>
        /// The current status of the drone (the status of the drone to the outside world)
        /// </summary>
        public  Status CurrentStatus { get;  private set; }

        /// <summary>
        /// The current state of the drone (internal state)
        /// </summary>
        public  State CurrentState { get;  private set; }

        public  State DesiredState { get;  private set; }

        public DroneState()
        {
            CurrentStatus = Status.NotReadyForWork;
            CurrentState = State.StartingUP;
            DesiredState = State.Ready;
        }

        /// <summary>
        /// Update the drones status
        /// </summary>
        /// <param name="desiredStatus"></param>
        public void UpdateState(State desiredStatus)
        {
            CurrentState = desiredStatus;
        }

        /// <summary>
        /// Update the drones state
        /// </summary>
        /// <param name="desiredStatus"></param>
        public void UpdateStatus(Status desiredStatus)
        {
            CurrentStatus = desiredStatus;
        }
    }

    /// <summary>
    /// Status values for the drone, these are only used for queen drone communication
    /// </summary>
    public enum Status
    {
        ReadyForWork,
        WaitingForWork,
        NotReadyForWork,
        Error
    }

    /// <summary>
    /// Valid states with in the drone
    /// </summary>
    public enum State
    {
        Ready,
        StartingUP,
        StartingTask,
        StoppingTask,
        CleaningUP,
        ResettingWorkspace,
        Restarting,
        ShuttingDown,
        ErrorFault,
        CheckForPackage,
        WaitingForPackage,
        WaitingForTask,
        Running
    }
}
