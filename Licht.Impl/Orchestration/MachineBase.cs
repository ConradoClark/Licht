using JetBrains.Annotations;
using Licht.Interfaces.Orchestration;

namespace Licht.Impl.Orchestration
{
    [PublicAPI]
    public abstract class MachineBase: IMachine
    {
        public abstract MachineStepResult RunStep();
        public object CurrentQueue { get; set; }
        public abstract void Cleanup();
    }
}
