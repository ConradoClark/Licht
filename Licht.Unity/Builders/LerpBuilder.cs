using System;
using System.Collections.Generic;
using System.Net.Http;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;

namespace Licht.Unity.Builders
{
    public class LerpBuilder
    {
        private readonly Action<float> _setter;
        private readonly Func<float> _getter;
        private float _target;
        private Func<float> _targetFn;
        private float _duration;
        private EasingYields.EasingFunction _easing;
        private ITime _timer;
        private Func<bool> _breakCondition;
        private bool _setTargetOnBreak;

        public LerpBuilder(Action<float> setter, Func<float> getter)
        {
            _setter = setter;
            _getter = getter;
        }

        public LerpBuilder Increase(float amount)
        {
            _target = _getter() + amount;
            _targetFn = null;
            return this;
        }

        public LerpBuilder Decrease(float amount)
        {
            _target = _getter() - amount;
            _targetFn = null;
            return this;
        }

        public LerpBuilder SetTarget(float target)
        {
            _target = target;
            _targetFn = null;
            return this;
        }

        public LerpBuilder SetTarget(Func<float> target)
        {
            _targetFn = target;
            return this;
        }

        public LerpBuilder Over(float seconds)
        {
            _duration = seconds;
            return this;
        }

        public LerpBuilder Easing(EasingYields.EasingFunction easingFunction)
        {
            _easing = easingFunction;
            return this;
        }

        public LerpBuilder UsingTimer(ITime timer)
        {
            _timer = timer;
            return this;
        }

        public IEnumerable<Action> Build()
        {
            if (_targetFn != null)
            {
                return EasingYields.Lerp(
                    _setter,
                    _getter,
                    _duration,
                    _targetFn,
                    _easing,
                    _timer ?? BasicToolbox.Instance.MainTimer,
                    _breakCondition,
                    _setTargetOnBreak
                );
            }
            return EasingYields.Lerp(
                _setter,
                _getter,
                _duration,
                _target,
                _easing,
                _timer ?? BasicToolbox.Instance.MainTimer,
                _breakCondition,
                _setTargetOnBreak
            );
        }

        public LerpBuilder BreakIf(Func<bool> predicate)
        {
            _breakCondition = predicate;
            return this;
        }

        public LerpBuilder EnsureTarget()
        {
            _setTargetOnBreak = true;
            return this;
        }
    }
}
