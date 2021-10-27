using System;
using System.Collections.Generic;
using System.Linq;

namespace Licht.Impl.Orchestration
{
    public static class BasicMachineryExtensions
    {
        public static BasicMachinery AddBasicMachine(this BasicMachinery basicMachinery, int priority, IEnumerable<Action> steps, Func<bool> breakCondition = null)
        {
            basicMachinery.AddMachines(new BasicMachine(priority, steps, breakCondition));
            return basicMachinery;
        }

        public static BasicMachinery AddBasicMachine(this BasicMachinery basicMachinery, int priority, IEnumerable<IEnumerable<Action>> steps, Func<bool> breakCondition = null)
        {
            basicMachinery.AddMachines(new BasicMachine(priority, steps, breakCondition));
            return basicMachinery;
        }

        public static IEnumerable<Action> AsEnumerable(this Action action)
        {
            return Enumerable.Repeat(action, 1);
        }

        public static IEnumerable<Action> Combine(this IEnumerable<Action> source, IEnumerable<Action> target)
        {
            using (var sourceEnumerator = source.GetEnumerator())
            using (var targetEnumerator = target.GetEnumerator())
            {
                while (sourceEnumerator.MoveNext() | targetEnumerator.MoveNext())
                {
                    yield return TimeYields.WaitOneFrame;
                }
            }
        }

        public static IEnumerable<Action> Then(this IEnumerable<Action> source, IEnumerable<Action> target)
        {
            using (var sourceEnumerator = source.GetEnumerator())
            using (var targetEnumerator = target.GetEnumerator())
            {
                while (sourceEnumerator.MoveNext())
                {
                    yield return TimeYields.WaitOneFrame;
                }

                while (targetEnumerator.MoveNext())
                {
                    yield return TimeYields.WaitOneFrame;
                }
            }
        }
    }
}
