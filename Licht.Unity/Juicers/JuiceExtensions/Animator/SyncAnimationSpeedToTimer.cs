using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Juicers.JuiceExtensions.Animator
{
    [AddComponentMenu("L. Runner: Sync AnimSpeed to Timer")]
    public class SyncAnimationSpeedToTimer : BaseGameRunner
    {
        [field:SerializeField]
        public UnityEngine.Animator Animator { get; private set; }

        protected override IEnumerable<IEnumerable<Action>> Handle()
        {
            while (ComponentEnabled)
            {
                Animator.speed = Timer.Multiplier;
                yield return TimeYields.WaitOneFrameX;
            }
        }
    }
}
