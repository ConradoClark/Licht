using System;
using JetBrains.Annotations;
using Licht.Interfaces.Orchestration;

namespace Licht.Impl.Orchestration
{
    [PublicAPI]
    public class WaitForConditionMachine : MachineBase
    {
        private readonly Func<bool> _condition;
        public WaitForConditionMachine(int priority, Func<bool> condition) : base(priority)
        {
            _condition = condition;
        }

        public override MachineStepResult RunStep()
        {
            return (_condition?.Invoke()).GetValueOrDefault() ? MachineStepResult.Skip : MachineStepResult.InternalWaiting;
        }

        public override void Cleanup()
        {
        }
    }
}
