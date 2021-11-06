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

        /// <summary>
        /// Starts a target co-routine when any of the sources are completed.
        /// Ends when all co-routines are completed
        /// </summary>
        /// <param name="onTargetStarted"></param>
        /// <param name="target"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static IEnumerable<Action> StartWhenAny(Action onTargetStarted, IEnumerable<Action> target, params IEnumerable<Action>[] sources)
        {
            var enumerators = sources.Select(s => s.GetEnumerator()).ToArray();

            while (enumerators.All(e => e.MoveNext()))
            {
                yield return TimeYields.WaitOneFrame;
            }

            enumerators = enumerators.Concat(new[] {target.GetEnumerator()}).ToArray();
            onTargetStarted();

            while (enumerators.Any(e => e.MoveNext()))
            {
                yield return TimeYields.WaitOneFrame;
            }

            foreach (var enumerator in enumerators)
            {
                enumerator.Dispose();
            }
        }

        public static IEnumerable<Action> StartWhenAny(Action onTargetStarted, Func<IEnumerable<Action>> target, params IEnumerable<Action>[] sources)
        {
            var enumerators = sources.Select(s => s.GetEnumerator()).ToArray();

            while (enumerators.All(e => e.MoveNext()))
            {
                yield return TimeYields.WaitOneFrame;
            }

            enumerators = enumerators.Concat(new[] { target().GetEnumerator() }).ToArray();
            onTargetStarted();

            while (enumerators.Any(e => e.MoveNext()))
            {
                yield return TimeYields.WaitOneFrame;
            }

            foreach (var enumerator in enumerators)
            {
                enumerator.Dispose();
            }
        }

        public static IEnumerable<Action> StartWhenAny(IEnumerable<Action> target, params IEnumerable<Action>[] sources)
        {
            return StartWhenAny(() => { }, target, sources);
        }

        public static IEnumerable<Action> StartWhenAny(Func<IEnumerable<Action>> target, params IEnumerable<Action>[] sources)
        {
            return StartWhenAny(() => { }, target, sources);
        }
    }
}
