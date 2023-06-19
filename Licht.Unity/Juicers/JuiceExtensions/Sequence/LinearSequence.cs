using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Juicers.JuiceExtensions.Sequence
{
    [AddComponentMenu("L!> Runner: LinearSequence")]
    public class LinearSequence : BaseGameRunner
    {
        [field:SerializeField]
        public BaseGameRunner[] Runners { get; private set; }
        protected override IEnumerable<IEnumerable<Action>> Handle()
        {
            foreach (var runner in Runners)
            {
                yield return runner.AsMachine().AsCoroutine().Until(() => !ComponentEnabled);
                if (!ComponentEnabled) break;
            }
        }
    }
}
