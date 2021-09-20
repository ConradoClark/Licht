using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Interfaces.Orchestration;

namespace Licht.Impl.Orchestration
{
    public abstract class MachineBase: IMachine
    {
        public int Priority { get; }
        public int AssignedPriority { get; set; }
        public abstract MachineStepResult RunStep();
        public object CurrentQueue { get; set; }

        protected MachineBase(int priority)
        {
            Priority = priority;
        }
    }
}
