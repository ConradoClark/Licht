using System;

namespace Licht.Interfaces.Orchestration
{
    public interface IMachine
    {
        MachineStepResult RunStep();
        object CurrentQueue { get; set; }
        void Cleanup();
    }
}
