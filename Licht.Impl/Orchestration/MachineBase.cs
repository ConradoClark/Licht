using JetBrains.Annotations;
using Licht.Interfaces.Orchestration;

namespace Licht.Impl.Orchestration
{
    [PublicAPI]
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

        public abstract void Cleanup();
    }
}
