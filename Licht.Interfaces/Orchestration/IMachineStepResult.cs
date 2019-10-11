using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Interfaces.Orchestration
{
    public enum MachineStepResult
    {
        QueueWaiting,
        InternalWaiting,
        Processing,
        Done,
        Skip
    }
}
