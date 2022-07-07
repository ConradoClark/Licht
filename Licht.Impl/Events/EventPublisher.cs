using System;
using Licht.Interfaces.Events;

namespace Licht.Impl.Events
{
    public class EventPublisher<TEvent, TObject> : IEventPublisher<TEvent, TObject>
    {
        public object Source { get; }
        private readonly Action<TEvent, TObject> _action;
        public EventPublisher(object source, Action<TEvent, TObject> action)
        {
            Source = source;
            _action = action;
        }
        public void PublishEvent(TEvent eventName, TObject eventObject)
        {
            _action(eventName, eventObject);
        }
    }

    public class VoidEventPublisher<TEvent, TObject> : IEventPublisher<TEvent, TObject>, IEventPublisher<TEvent>
    {
        public object Source { get; }
        void IEventPublisher<TEvent, TObject>.PublishEvent(TEvent eventName, TObject eventObject)
        {
            _action(eventName, eventObject);
        }

        private readonly Action<TEvent, TObject> _action;
        public VoidEventPublisher(object source, Action<TEvent, TObject> action)
        {
            Source = source;    
            _action = action;
        }
        public void PublishEvent(TEvent eventName)
        {
            (this as IEventPublisher<TEvent, TObject>).PublishEvent(eventName, default);
        }
    }
}
