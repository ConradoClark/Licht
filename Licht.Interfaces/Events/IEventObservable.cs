using System;

namespace Licht.Interfaces.Events
{
    public interface IEventObservable<in TEventType, out TEventObject>
    {
        void ObserveEvent(TEventType eventName, Action<TEventObject> onEvent);
        void StopObservingEvent(TEventType eventName, Action<TEventObject> onEvent);
        void StopObservingAllEvents();
    }
}
