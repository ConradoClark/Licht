using System;
using System.Runtime.CompilerServices;
using Licht.Impl.Orchestration;
using Licht.Impl.Time;
using NUnit.Framework;

namespace Licht.Test
{
    [TestFixture]
    public class EasingTest
    {
        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void LinearEaseIn()
        {
            var timer = new DefaultTimer(() => 1000d, 1);
            timer.Activate();

            var f = new StrongBox<float>(0);
            var machinery = new BasicMachinery<int>(0);

            var lerp = EasingYields.Lerp(x => f.Value = x,
                () => f.Value,
                5, 5, EasingYields.EasingFunction.Linear,
                timer);

            machinery.AddMachines(1, new BasicMachine(lerp));

            var result = new[] {0, 1, 2, 3, 4, 5};
            for (var i = 0; i < 6;i++)
            {                
                machinery.Update();
                timer.Update();
                Assert.IsTrue(Math.Abs(result[i] - f.Value) < 0.001f);
            }
        }
    }
}
