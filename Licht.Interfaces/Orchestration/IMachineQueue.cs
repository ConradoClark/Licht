namespace Licht.Interfaces.Orchestration
{
    public interface IMachineQueue
    {
        void Enqueue(IMachine machine);
        IMachine Dequeue();
        IMachine Peek();
        void Cancel(IMachine machine);
        bool IsEmpty { get; }
    }
}
