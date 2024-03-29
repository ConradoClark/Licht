﻿using System;
using Licht.Impl.Time;
using NUnit.Framework;

namespace Licht.Test
{
    [TestFixture]    
    public class TimeTests
    {
        const float Margin = 0.001f;

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void DefaultFloatTimer_NoMultiplier_StopAfter1Second()
        {
            DefaultTimer timer = new DefaultTimer(() => 1000f, 1);
            timer.Update();
            timer.Deactivate();
            Assert.IsTrue(Math.Abs(timer.TotalElapsedTimeInMilliseconds - 1000d) < Margin);
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void DefaultFloatTimer_2Multiplier_StopAfter1Second()
        {
            DefaultTimer timer = new DefaultTimer(() => 1000f, 1) { Multiplier = 2 };
            timer.Update();
            timer.Deactivate();
            Assert.IsTrue(Math.Abs(timer.TotalElapsedTimeInMilliseconds - 1000d) < Margin);
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void DefaultFloatTimer_NoMultiplier_StopAfter1Second_Reset_StartAndStopAfter1Second()
        {
            DefaultTimer timer = new DefaultTimer(() => 1000f, 1);
            timer.Activate();
            timer.Update();
            timer.PerformReset();
            timer.Update();
            Assert.IsTrue(Math.Abs(timer.TotalElapsedTimeInMilliseconds - 1000d) < Margin);
        }
    }
}
