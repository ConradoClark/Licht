using System;
using System.Collections.Generic;
using System.Text;
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
}
