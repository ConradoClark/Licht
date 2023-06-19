using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

namespace Licht.Unity.Effects
{
    [AddComponentMenu("L. Pooling: Expire After Duration")]
    public class ExpireAfterDuration : BaseGameAgent
    {
        public PooledObject Poolable { get; private set; }

        [field: SerializeField]
        public float DurationInSeconds { get; set; }

        public override void Init()
        {
            base.Init();
            if (Poolable == null && Actor.TryGetCustomObject<PooledObject>(out var pooledComponent))
            {
                Poolable = pooledComponent;
            }
        }

        override protected IEnumerable<IEnumerable<Action>> Handle()
        {
            yield return TimeYields.WaitSeconds(Timer, DurationInSeconds);
            Poolable.Release();
        }
    }
}
