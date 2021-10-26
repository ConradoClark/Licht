namespace Licht.Interfaces.Orchestration
{
    public interface IMachine
    {
        int Priority { get; }
        int AssignedPriority { get; set; }
        MachineStepResult RunStep();
        object CurrentQueue { get; set; }
        void Cleanup();
    }
}
