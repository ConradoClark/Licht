﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Interfaces.Math;
using Licht.Interfaces.Time;

namespace Licht.Impl.Orchestration
{
    public static class TimeYields
    {
        public static Action WaitOneFrame => () => { };

        public static IEnumerable<Action> WaitMilliseconds(ITime timer, double ms)
        {
            var time = 0d;
            while (time < ms)
            {
                yield return WaitOneFrame;
                time += timer.UpdatedTimeInMilliseconds;
            }
        }

        public static IEnumerable<Action> WaitSeconds(ITime timer, double seconds)
        {
            return WaitMilliseconds(timer, seconds * 1000d);
        }
    }
}
