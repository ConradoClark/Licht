using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Interfaces.Events
{
    public interface IEvent<out T>
    {
        T Object { get; }
        string EventName { get; }
    }
}