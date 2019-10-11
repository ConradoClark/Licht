namespace Licht.Interfaces.Events
{
    public interface IEvent<out T>
    {
        T Object { get; }
        string EventName { get; }
    }
}