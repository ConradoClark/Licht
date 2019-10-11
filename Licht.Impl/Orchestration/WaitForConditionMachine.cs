﻿using System;
using Licht.Interfaces.Orchestration;

namespace Licht.Impl.Orchestration
{
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
    }
}
