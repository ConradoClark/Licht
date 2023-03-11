using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Licht.Interfaces.Orchestration;

namespace Licht.Impl.Orchestration
{
    [PublicAPI]
    public class UniqueMachine : MachineBase
    {
        private static Dictionary<string, bool> _uniqueChecker = new Dictionary<string, bool>();
        private bool _started;
        private readonly IEnumerator<Action> _actionEnumerator;
        private readonly Func<bool> _breakCondition;
        private readonly string _callerName;
        private readonly string _identifier;
        private readonly UniqueMachineBehaviour _behaviour;

        public enum UniqueMachineBehaviour
        {
            Cancel,
            Wait,
            Replace,
        }

        public UniqueMachine(string identifier, UniqueMachineBehaviour behaviour, IEnumerable<Action> steps, Func<bool> breakCondition = null, string callerName = "")
        {
            _identifier = identifier;
            _actionEnumerator = steps.GetEnumerator();
            _breakCondition = breakCondition;
            _callerName = callerName;
            _behaviour = behaviour;
        }

        public UniqueMachine(string identifier, UniqueMachineBehaviour behaviour, IEnumerable<IEnumerable<Action>> steps, Func<bool> breakCondition = null, string callerName = "")
        {
            _identifier = identifier;
            _actionEnumerator = steps.SelectMany(s => s).GetEnumerator();
            _breakCondition = breakCondition;
            _callerName = callerName;
            _behaviour = behaviour;
        }

        public UniqueMachine(string identifier, UniqueMachineBehaviour behaviour, Action step, Func<bool> breakCondition = null, string callerName = "")
        {
            _identifier = identifier;
            _actionEnumerator = Enumerable.Repeat(step, 1).GetEnumerator();
            _breakCondition = breakCondition;
            _callerName = callerName;
            _behaviour = behaviour;
        }

        public override MachineStepResult RunStep()
        {
            try
            {
                if (!_started)
                {
                    if (_uniqueChecker.ContainsKey(_identifier))
                    {
                        switch (_behaviour)
                        {
                            case UniqueMachineBehaviour.Cancel: return MachineStepResult.Done;
                            case UniqueMachineBehaviour.Replace:
                                _uniqueChecker[_identifier] = false;
                                return MachineStepResult.Processing;
                            case UniqueMachineBehaviour.Wait:
                                return MachineStepResult.Processing;
                        }
                    }

                    _uniqueChecker[_identifier] = true;

                    _actionEnumerator.MoveNext();
                    _started = true;
                }

                _actionEnumerator.Current?.Invoke();
                
                if (!_uniqueChecker[_identifier] || (_breakCondition != null && _breakCondition()))
                {
                    _uniqueChecker.Remove(_identifier);
                    return MachineStepResult.Done;
                }

                var moveNext = _actionEnumerator.MoveNext();

                if (!moveNext)
                {
                    _uniqueChecker.Remove(_identifier);
                    return MachineStepResult.Done;
                }

                return MachineStepResult.Processing;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in member {_callerName}", ex);
            }
        }

        public override void Cleanup()
        {
            if (_uniqueChecker.ContainsKey(_identifier))
            {
                _uniqueChecker.Remove(_identifier);
            }
            _actionEnumerator.Dispose();
        }
    }
}
