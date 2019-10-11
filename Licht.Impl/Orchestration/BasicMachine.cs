using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Interfaces.Orchestration;

namespace Licht.Impl.Orchestration
{
    public class BasicMachine : MachineBase
    {
        private bool _started;
        private readonly IEnumerator<Action> _actionEnumerator;

        public BasicMachine(int priority, IEnumerable<Action> steps) : base(priority)
        {
            _actionEnumerator = steps.GetEnumerator();
        }

        public BasicMachine(int priority, Action step) : base(priority)
        {
            _actionEnumerator = Enumerable.Repeat(step, 1).GetEnumerator();
        }

        public override MachineStepResult RunStep()
        {
            if (!_started)
            {
                _actionEnumerator.MoveNext();
                _started = true;
            }

            _actionEnumerator.Current?.Invoke();
            return _actionEnumerator.MoveNext() ? MachineStepResult.Processing : MachineStepResult.Done;
        }

    }
}
