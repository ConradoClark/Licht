using System.Collections.Generic;
using System.Linq;
using Licht.Interfaces.Orchestration;

namespace Licht.Impl.Orchestration
{
    public class PriorityQueue : IMachineQueue
    {
        private class PriorityComparer : IComparer<IMachine>
        {
            public int Compare(IMachine x, IMachine y)
            {
                return (x?.AssignedPriority.CompareTo(y?.AssignedPriority)).GetValueOrDefault();
            }
        }

        private readonly SortedSet<IMachine> _queue = new SortedSet<IMachine>(new PriorityComparer());
        public void Enqueue(IMachine machine)
        {
            _queue.Add(machine);
        }

        public IMachine Dequeue()
        {
            var machine = _queue.First();
            _queue.Remove(machine);
            return machine;
        }

        public IMachine Peek()
        {
            return _queue.First();
        }

        public void Cancel(IMachine machine)
        {
            _queue.Remove(machine);
        }

        public bool IsEmpty => _queue.Count == 0;
    }
}
