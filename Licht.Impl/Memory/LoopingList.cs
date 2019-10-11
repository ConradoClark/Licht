using Licht.Interfaces.Memory;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Licht.Impl.Memory
{
    [PublicAPI]
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
                _currentIndex = _currentIndex == 0 ? Math.Max(0, Count - 1) : (_currentIndex - 1) % Count;
                return Current;
            }
        }
    }
}