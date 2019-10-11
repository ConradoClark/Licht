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
