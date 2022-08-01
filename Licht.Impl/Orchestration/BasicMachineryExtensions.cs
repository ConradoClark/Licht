using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Licht.Interfaces.Time;

namespace Licht.Impl.Orchestration
{
    public static class BasicMachineryExtensions
    {
        public static BasicMachinery<TKey> AddBasicMachine<TKey>(this BasicMachinery<TKey> basicMachinery, TKey layer, IEnumerable<Action> steps, Func<bool> breakCondition = null)
        {
            basicMachinery.AddMachines(layer, new BasicMachine(steps, breakCondition));
            return basicMachinery;
        }

        public static BasicMachinery<TKey> AddBasicMachine<TKey>(this BasicMachinery<TKey> basicMachinery, TKey layer, IEnumerable<IEnumerable<Action>> steps, Func<bool> breakCondition = null)
        {
            basicMachinery.AddMachines(layer, new BasicMachine(steps, breakCondition));
            return basicMachinery;
        }

        public static BasicMachinery<TKey> AddBasicMachine<TKey>(this BasicMachinery<TKey> basicMachinery, IEnumerable<Action> steps, Func<bool> breakCondition = null)
        {
            return AddBasicMachine(basicMachinery, default, steps, breakCondition);
        }

        public static BasicMachinery<TKey> AddBasicMachine<TKey>(this BasicMachinery<TKey> basicMachinery, IEnumerable<IEnumerable<Action>> steps, Func<bool> breakCondition = null)
        {
            return AddBasicMachine(basicMachinery, default, steps, breakCondition);
        }

        public static BasicMachinery<TKey> AddUniqueMachine<TKey>(this BasicMachinery<TKey> basicMachinery, string identifier, UniqueMachine.UniqueMachineBehaviour behaviour, IEnumerable<Action> steps, Func<bool> breakCondition = null)
        {
            basicMachinery.AddMachines(default, new UniqueMachine(identifier, behaviour, steps, breakCondition));
            return basicMachinery;
        }

        public static BasicMachinery<TKey> AddUniqueMachine<TKey>(this BasicMachinery<TKey> basicMachinery, string identifier, UniqueMachine.UniqueMachineBehaviour behaviour, IEnumerable<IEnumerable<Action>> steps, Func<bool> breakCondition = null)
        {
            basicMachinery.AddMachines(default, new UniqueMachine(identifier, behaviour, steps, breakCondition));
            return basicMachinery;
        }

        public static IEnumerable<Action> AsCoroutine(this IEnumerable<IEnumerable<Action>> routine)
        {
            return routine.SelectMany(action => action);
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

        public static IEnumerable<Action> Combine(this IEnumerable<IEnumerable<Action>> source, IEnumerable<IEnumerable<Action>> target)
        {
            return source.AsCoroutine().Combine(target.AsCoroutine());
        }

        public static IEnumerable<Action> Then(this IEnumerable<IEnumerable<Action>> source, IEnumerable<IEnumerable<Action>> target)
        {
            return source.AsCoroutine().Then(target.AsCoroutine());
        }

        public static IEnumerable<Action> CombineAfterSeconds(this IEnumerable<IEnumerable<Action>> source, float seconds,
            ITimer timer, IEnumerable<IEnumerable<Action>> target)
        {
            return source.AsCoroutine().Combine(TimeYields.WaitSeconds(timer, seconds).Then(target.AsCoroutine()));
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

            enumerators = enumerators.Concat(new[] { target.GetEnumerator() }).ToArray();
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
