using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Core
{
    
    public enum Status
    {
        
        ReadyForWork,
        NotReadyForWork

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
        ErrorFault
    }

}
