using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Interfaces.Memory
{
    public interface IEnumerableCollection<out T>
    {
        T Current { get; }
        T Next { get; }
    }
}
