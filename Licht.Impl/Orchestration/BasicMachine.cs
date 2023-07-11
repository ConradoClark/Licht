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
        private IEnumerator<Action> _actionEnumerator;
        private readonly Lazy<IEnumerator<Action>> _actionEnumeratorLazy;
        private readonly bool _lazy;
        private readonly Func<bool> _breakCondition;
        private readonly string _callerName;

        public BasicMachine(IEnumerable<Action> steps, Func<bool> breakCondition = null, string callerName = "")
        {
            _actionEnumerator = steps.GetEnumerator();
            _breakCondition = breakCondition;
            _callerName = callerName;
        }

        public BasicMachine(IEnumerable<IEnumerable<Action>> steps, Func<bool> breakCondition = null, string callerName = "")
        {
            _actionEnumerator = steps.SelectMany(s => s).GetEnumerator();
            _breakCondition = breakCondition;
            _callerName = callerName;
        }

        public BasicMachine(Action step, Func<bool> breakCondition = null, string callerName = "")
        {
            _actionEnumerator = Enumerable.Repeat(step, 1).GetEnumerator();
            _breakCondition = breakCondition;
            _callerName = callerName;
        }

        public BasicMachine(Func<IEnumerable<IEnumerable<Action>>> steps, Func<bool> breakCondition = null, string callerName = "")
        {
            _actionEnumeratorLazy = new Lazy<IEnumerator<Action>>(() => steps().SelectMany(s => s).GetEnumerator());
            _lazy = true;
            _breakCondition = breakCondition;
            _callerName = callerName;
        }

        public override MachineStepResult RunStep()
        {
            try
            {
                if (!_started)
                {
                    if (_lazy)
                    {
                        _actionEnumerator = _actionEnumeratorLazy.Value;
                    }
                    _actionEnumerator.MoveNext();
                    _started = true;
                }

                _actionEnumerator.Current?.Invoke();
                if (_breakCondition != null && _breakCondition()) return MachineStepResult.Done;
                return _actionEnumerator.MoveNext() ? MachineStepResult.Processing : MachineStepResult.Done;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in member {_callerName}", ex);
            }
        }

        public override void Cleanup()
        {
            _actionEnumerator?.Dispose();
        }
    }
}
