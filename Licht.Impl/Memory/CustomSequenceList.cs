using System.Collections.Generic;
using Licht.Interfaces.Memory;

namespace Licht.Impl.Memory
{
    public class CustomSequenceList<T> : List<T>, IEnumerableCollection<T>
    {
        private int _currentIndex;
        private readonly IEnumerator<int> _sequenceEnumerator;

        public CustomSequenceList(IEnumerable<int> sequence)
        {
            _sequenceEnumerator = sequence.GetEnumerator();
        }
        public T Current => _currentIndex >= 0 && Count > _currentIndex ? this[_currentIndex] : default(T);

        public T Next
        {
            get
            {
                if (Count == 0) return default(T);
                
                _sequenceEnumerator.MoveNext();
                _currentIndex = _sequenceEnumerator.Current;
                return Current;
            }
        }
    }
}
