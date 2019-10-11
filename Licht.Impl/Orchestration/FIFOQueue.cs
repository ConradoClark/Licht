using System.Collections.Generic;
using System.Linq;
using Licht.Interfaces.Orchestration;

namespace Licht.Impl.Orchestration
{
    public class FIFOQueue : IMachineQueue
    {
        private Queue<IMachine> _queue = new Queue<IMachine>();

        public void Enqueue(IMachine machine)
        {
            if (machine == null) return;
            _queue.Enqueue(machine);
        }

        public IMachine Dequeue()
        {
            return _queue.Dequeue();
        }

        public IMachine Peek()
        {
            return _queue.Peek();
        }

        public void Cancel(IMachine machine)
        {
            _queue = new Queue<IMachine>(_queue.Except(new[] { machine }));
        }

        public bool IsEmpty => _queue.Count == 0;
    }
}
