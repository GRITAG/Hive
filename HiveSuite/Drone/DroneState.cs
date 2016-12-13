using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Drone
{
    public class DroneState
    {
        public  Status CurrentStatus { get;  private set; }

        public  State CurrentState { get;  private set; }

        public  State DesiredState { get;  private set; }

        public DroneState()
        {
            CurrentStatus = Status.NotReadyForWork;
            CurrentState = State.StartingUP;
            DesiredState = State.Ready;
        }

        public void UpdateState(State desiredStatus)
        {
            CurrentState = desiredStatus;
        }

        public void UpdateStatus(Status desiredStatus)
        {
            CurrentStatus = desiredStatus;
        }
    }

    public enum Status
    {

        ReadyForWork,
        WaitingForWork,
        NotReadyForWork,

    }

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
        WaitingForWork,
        Running
    }
}
