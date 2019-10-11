using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Interfaces.Memory
{
    public interface IReversibleCollection<out T> : IEnumerableCollection<T>
    {
        T Previous { get; }
    }
}
