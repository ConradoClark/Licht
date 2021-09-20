using Licht.Interfaces.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Impl.Memory
{
    public class LoopingList<T> : List<T>, IReversibleCollection<T>
    {
        private int _currentIndex;

        public T Current => Count > _currentIndex ? this[_currentIndex] : default(T);

        public T Next
        {
            get
            {
                _currentIndex = (_currentIndex + 1) % Count;
                return Current;
            }
        }

        public T Previous
        {
            get
            {
                _currentIndex = _currentIndex == 0 ? Math.Max(0, Count - 1) : ((_currentIndex - 1) % Count);
                return Current;
            }
        }
    }
}