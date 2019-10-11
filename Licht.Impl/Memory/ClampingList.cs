using System;
using System.Collections.Generic;
using Licht.Interfaces.Memory;

namespace Licht.Impl.Memory
{
    public class ClampingList<T> : List<T>, IReversibleCollection<T>
    {
        private int _currentIndex;
        public T Current => Count > 0 ? this[_currentIndex] : default(T);

        public T Next
        {
            get
            {
                _currentIndex = Math.Min(_currentIndex + 1, Count - 1);
                return Current;
            }
        }

        public T Previous
        {
            get
            {
                _currentIndex = _currentIndex == 0 ? 0 : _currentIndex - 1;
                return Current;
            }
        }
    }
}
