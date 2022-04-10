using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Licht.Interfaces.Orchestration;

namespace Licht.Impl.Orchestration
{
    [PublicAPI]
    public class BasicMachine : MachineBase
    {
        private bool _started;
        private readonly IEnumerator<Action> _actionEnumerator;
        private readonly Func<bool> _breakCondition;

        public BasicMachine( IEnumerable<Action> steps, Func<bool> breakCondition = null)
        {
            _actionEnumerator = steps.GetEnumerator();
            _breakCondition = breakCondition;
        }

        public BasicMachine(IEnumerable<IEnumerable<Action>> steps, Func<bool> breakCondition = null)
        {
            _actionEnumerator = steps.SelectMany(s => s).GetEnumerator();
            _breakCondition = breakCondition;
        }

        public BasicMachine(Action step, Func<bool> breakCondition = null)
        {
            _actionEnumerator = Enumerable.Repeat(step, 1).GetEnumerator();
            _breakCondition = breakCondition;
        }

        public override MachineStepResult RunStep()
        {
            if (!_started)
            {
                _actionEnumerator.MoveNext();
                _started = true;
            }

            _actionEnumerator.Current?.Invoke();
            if (_breakCondition != null && _breakCondition()) return MachineStepResult.Done;
            return _actionEnumerator.MoveNext() ? MachineStepResult.Processing : MachineStepResult.Done;
        }

        public override void Cleanup()
        {
            _actionEnumerator.Dispose();
        }
    }
}
