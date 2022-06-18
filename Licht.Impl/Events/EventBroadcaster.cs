using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Interfaces.Events;

namespace Licht.Impl.Events
{
    public static class EventExtensions
    {

        private static readonly Dictionary<object, object> EventFn = new Dictionary<object, object>();
        public static void ObserveEvent<TEventType>(this object obj, TEventType @event,
            Action onEvent)
        {
            var fn = new Action<object>(_ => onEvent());
            EventFn[onEvent] = fn;

            EventBroadcaster<TEventType, object>.Instance<TEventType, object>()
                .ObserveEvent(@event, fn);
        }

        public static void StopObservingEvent<TEventType>(this object obj, TEventType @event,
            Action onEvent)
        {
            EventBroadcaster<TEventType, object>.Instance<TEventType, object>()
                .StopObservingEvent(@event, EventFn.ContainsKey(onEvent) ?
                    EventFn[onEvent] as Action<object> : _ => onEvent());
        }

        public static void ObserveEvent<TEventType, TEventObject>(this object obj, TEventType @event,
            Action<TEventObject> onEvent)
        {
            EventBroadcaster<TEventType, TEventObject>.Instance<TEventType, TEventObject>()
                .ObserveEvent(@event, onEvent);
        }

        public static void StopObservingEvent<TEventType, TEventObject>(this object obj, TEventType @event,
            Action<TEventObject> onEvent)
        {
            EventBroadcaster<TEventType, TEventObject>.Instance<TEventType, TEventObject>()
                .StopObservingEvent(@event, onEvent);
        }

        public static IEventPublisher<TEventType> RegisterAsEventPublisher<TEventType>(this object publisher)
        {
            return EventBroadcaster<TEventType, object>.Instance<TEventType, object>()
                .RegisterVoidPublisher(publisher);
        }

        public static IEventPublisher<TEventType, TEventObject> RegisterAsEventPublisher<TEventType, TEventObject>(this object publisher)
        {
            return EventBroadcaster<TEventType, TEventObject>.Instance<TEventType, TEventObject>()
                .RegisterPublisher(publisher);
        }

        public static void UnregisterAsEventPublisher<TEventType, TEventObject>(this object publisher)
        {
            EventBroadcaster<TEventType, TEventObject>.Instance<TEventType, TEventObject>()
                 .UnregisterPublisher(publisher);
        }
    }

    public class EventBroadcaster<TEventType, TEventObject> : IEventObservable<TEventType, TEventObject>
    {

        private class BroadcasterRegistry
        {
            public Type EventType;
            public Type ObjectType;
            public object Broadcaster;
        }

        private struct EventRef
        {
            public TEventType Key;
            public Action<TEventObject> Ref;
        }

        private static readonly List<BroadcasterRegistry> Broadcasters = new List<BroadcasterRegistry>();
        public static EventBroadcaster<T1, T2> Instance<T1, T2>()
        {
            var register = Broadcasters.FirstOrDefault(b => b.EventType == typeof(T1) &&
                                                       b.ObjectType == typeof(T2));

            if (register == null)
            {
                register = new BroadcasterRegistry
                {
                    EventType = typeof(T1),
                    ObjectType = typeof(T2),
                    Broadcaster = new EventBroadcaster<T1, T2>()
                };
                Broadcasters.Add(register);
            }

            return register.Broadcaster as EventBroadcaster<T1, T2>;
        }

        private readonly List<IEventPublisher<TEventType, TEventObject>> _publishers =
            new List<IEventPublisher<TEventType, TEventObject>>();

        private readonly Dictionary<EventRef, Action<TEventType, TEventObject>> _eventRefs =
            new Dictionary<EventRef, Action<TEventType, TEventObject>>();

        public void ObserveEvent(TEventType eventName, Action<TEventObject> onEvent)
        {
            if (onEvent == null) return;

            var eventRef = new EventRef
            {
                Key = eventName,
                Ref = onEvent,
            };

            _eventRefs[eventRef] = (type, obj) =>
            {
                if (!type.Equals(eventName)) return;
                onEvent(obj);
            };

            ObjectEvent += _eventRefs[eventRef];
        }

        public void StopObservingEvent(TEventType eventName, Action<TEventObject> onEvent)
        {
            var eventRef = new EventRef
            {
                Key = eventName,
                Ref = onEvent,
            };
            if (!_eventRefs.ContainsKey(eventRef)) return;

            ObjectEvent -= _eventRefs[eventRef];
        }

        public void StopObservingAllEvents()
        {
            ObjectEvent = null;
        }

        public IEventPublisher<TEventType, TEventObject> RegisterPublisher(object publisher)
        {
            var eventPublisher = new EventPublisher<TEventType, TEventObject>(publisher,
                (@event, @object) => ObjectEvent?.Invoke(@event, @object));
            _publishers.Add(eventPublisher);
            return eventPublisher;
        }

        public IEventPublisher<TEventType> RegisterVoidPublisher(object publisher)
        {
            var eventPublisher = new VoidEventPublisher<TEventType, TEventObject>(publisher,
                (@event, @object) => ObjectEvent?.Invoke(@event, @object));
            _publishers.Add(eventPublisher);
            return eventPublisher;
        }

        public void UnregisterPublisher(object publisher)
        {
            var removeList =
                _publishers.Where(p => p.Source == publisher).ToArray();

            foreach (var pub in removeList)
            {
                _publishers.Remove(pub);
            }
        }

        public event Action<TEventType, TEventObject> ObjectEvent;
    }
}
