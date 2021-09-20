using JetBrains.Annotations;
using Licht.Interfaces.Events;

namespace Licht.Impl.Events
{
    [PublicAPI]
    public class GenericEvent<T> : IEvent<T>
    {
        public T Object { get; }
        public string EventName { get; }

        public GenericEvent(T obj, string eventName)
        {
            Object = obj;
            EventName = eventName;
        }
    }
}
