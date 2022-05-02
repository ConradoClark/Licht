using System;
using System.Collections.Generic;
using Licht.Impl.Debug;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;

namespace Licht.Unity.Builders
{
    public class LerpBuilder
    {
        public static ITime DefaultTimer;
        private readonly Action<float> _setter;
        private readonly Func<float> _getter;
        private float _target;
        private Func<float> _targetFn;
        private float _duration;
        private EasingYields.EasingFunction _easing;
        private ITime _timer;
        private Func<bool> _breakCondition;
        private bool _setTargetOnBreak = true;
        private bool _fromOrigin = false;
        private bool _fixedTarget = false;

        public LerpBuilder(Action<float> setter, Func<float> getter)
        {
            _setter = setter;
            _getter = getter;
        }

        public LerpBuilder FromOrigin()
        {
            _fromOrigin = true;
            return this;
        }

        public LerpBuilder FromUpdatedValues()
        {
            _fromOrigin = false;
            return this;
        }

        public LerpBuilder Increase(float amount)
        {
            _fixedTarget = false;
            _target = _fromOrigin ? _getter() + amount : amount;
            _targetFn = null;
            return this;
        }

        public LerpBuilder Decrease(float amount)
        {
            _fixedTarget = false;
            _target = _fromOrigin ? _getter() - amount : -amount;
            _targetFn = null;
            return this;
        }

        public LerpBuilder SetTarget(float target)
        {
            _fixedTarget = true;
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

        private LerpBuilder Clone()
        {
            return new LerpBuilder(_setter, _getter)
            {
                _target = _target,
                _targetFn = _targetFn,
                _duration = _duration,
                _easing = _easing,
                _timer = _timer,
                _breakCondition = _breakCondition,
                _setTargetOnBreak = _setTargetOnBreak,
                _fromOrigin = _fromOrigin,
                _fixedTarget = _fixedTarget,
            };
        }

        public IEnumerable<Action> Build()
        {
            var clone = Clone();
            return Build(clone);
        }

        private IEnumerable<Action> Build(LerpBuilder clone)
        {
            yield return TimeYields.WaitOneFrame;

            if (_targetFn != null)
            {
                var lerpFn = EasingYields.Lerp(
                    clone._setter,
                    clone._getter,
                    clone._duration,
                    clone._targetFn,
                    clone._easing,
                    clone._timer ?? DefaultTimer,
                    clone._breakCondition,
                    clone._setTargetOnBreak,
                    immediate: true
                );

                foreach (var step in lerpFn)
                {
                    yield return step;
                }
            }

            if (!clone._fixedTarget && !clone._fromOrigin)
            {
                clone._target += clone._getter();
            }

            var lerp = EasingYields.Lerp(
                clone._setter,
                clone._getter,
                clone._duration,
                clone._target,
                clone._easing,
                clone._timer ?? DefaultTimer,
                clone._breakCondition,
                clone._setTargetOnBreak,
                immediate: true
            );

            foreach (var step in lerp)
            {
                yield return step;
            }
        }

        public LerpBuilder BreakIf(Func<bool> predicate, bool setTargetOnBreak = true)
        {
            _breakCondition = predicate;
            _setTargetOnBreak = setTargetOnBreak;
            return this;
        }
    }
}
