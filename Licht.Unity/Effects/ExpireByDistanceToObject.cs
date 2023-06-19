using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.Effects
{
    [AddComponentMenu("L. Pooling: Expire By Distance (Obj)")]
    public class ExpireByDistanceToObject : BaseGameAgent
    {
        public PooledObject Poolable { get; private set; }

        [field:SerializeField]
        [field:CustomLabel("Select this if you want to use another Transform to compare the distance.")]
        public bool UseCustomReference { get; private set; }

        [field:ShowWhen(nameof(UseCustomReference))]
        [field: SerializeField]
        public Transform Reference { get; set; }

        [field:CustomLabel("Target object's Transform")]
        [field: SerializeField]
        public Transform Target { get; set; }

        [field: SerializeField]
        public float Distance { get; set; }

        private bool _effectEnded;
        public override void Init()
        {
            base.Init();
            if (Poolable == null && Actor.TryGetCustomObject<PooledObject>(out var pooledComponent))
            {
                Poolable = pooledComponent;
            }
        }

        override protected void OnEnable()
        {
            _effectEnded = false;
            base.OnEnable();
        }

        override protected IEnumerable<IEnumerable<Action>> Handle()
        {
            var reference = UseCustomReference ? Reference : transform;
            if (!_effectEnded && !(Vector2.Distance(reference.position, Target.position) < Distance))
            {
                Poolable.Release();
                _effectEnded = true;
                yield break;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
