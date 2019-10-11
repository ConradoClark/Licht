using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Numbers;
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
            var machinery = new BasicMachinery();

            var lerp = EasingYields.Lerp(x => f.Value = x,
                () => f.Value,
                5, 5, EasingYields.EasingFunction.Linear,
                timer);

            machinery.AddMachines(new BasicMachine(1, lerp));

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
