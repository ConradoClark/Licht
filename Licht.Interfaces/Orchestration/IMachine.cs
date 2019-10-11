using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Interfaces.Orchestration
{
    public interface IMachine
    {
        int Priority { get; }
        int AssignedPriority { get; set; }
        MachineStepResult RunStep();
        object CurrentQueue { get; set; }
    }
}
