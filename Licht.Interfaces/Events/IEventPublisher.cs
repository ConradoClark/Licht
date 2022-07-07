namespace Licht.Interfaces.Events
{
    public interface IEventPublisher<in TEventType, in TEventObject>
    {
        object Source { get; }
        void PublishEvent(TEventType eventName, TEventObject eventObject);
    }

    public interface IEventPublisher<in TEventType>
    {
        object Source { get; }
        void PublishEvent(TEventType eventName);
    }
}
