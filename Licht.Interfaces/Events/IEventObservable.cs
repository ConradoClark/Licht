using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Interfaces.Events
{
    public interface IEventObservable<in TEventType, out TEventObject>
    {
        void ObserveEvent(TEventType eventName, Action<TEventObject> onEvent);
        void StopObservingEvent(TEventType eventName, Action<TEventObject> onEvent);
        void StopObservingAllEvents();
    }
}
