using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

namespace Licht.Unity.Effects
{
    public class ExpireAfterDuration : BaseGameRunner
    {
        [field: SerializeField]
        public EffectPoolable Poolable { get; private set; }

        [field: SerializeField]
        public float DurationInSeconds { get; private set; }

        override protected void OnEnable()
        {
            base.OnEnable();
        }

        override protected IEnumerable<IEnumerable<Action>> Handle()
        {
            yield return TimeYields.WaitSeconds(Timer, DurationInSeconds);
            Poolable.EndEffect();
        }
    }
}
