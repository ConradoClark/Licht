using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Licht.Interfaces.Time;

namespace Licht.Impl.Orchestration
{
    [PublicAPI]
    public static class TimeYields
    {
        public static Action WaitOneFrame => () => { };

        public static IEnumerable<Action> WaitOneFrameX => Enumerable.Repeat<Action>(() => { }, 1);

        public static IEnumerable<Action> WaitMilliseconds(ITime timer, double ms, Action<double> step = null, Func<bool> breakCondition = null)
        {
            var time = 0d;
            while (time < ms)
            {
                if (breakCondition != null && breakCondition()) yield break;
                step?.Invoke(time);
                yield return WaitOneFrame;
                time += timer.UpdatedTimeInMilliseconds;
            }
        }

        public static IEnumerable<Action> WaitSeconds(ITime timer, double seconds, Action<double> step = null, Func<bool> breakCondition = null)
        {
            return WaitMilliseconds(timer, seconds * 1000d, step, breakCondition);
        }
    }
}
