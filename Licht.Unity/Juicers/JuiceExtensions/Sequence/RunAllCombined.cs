using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Juicers.JuiceExtensions.Sequence
{
    [AddComponentMenu("L. Runner: RunAllCombined")]
    public class RunAllCombined : BaseGameRunner
    {
        [field: SerializeField]
        public BaseGameRunner[] Runners { get; private set; }
        protected override IEnumerable<IEnumerable<Action>> Handle()
        {
            if (Runners.Length == 0) yield break;

            var accRunner = Runners[0].AsMachine().AsCoroutine();
            for (var i = 1; i < Runners.Length; i++)
            {
                var runner = Runners[i];
                accRunner = accRunner.Combine(runner.AsMachine().AsCoroutine());
            }

            yield return accRunner.Until(() => !ComponentEnabled);
        }
    }
}
