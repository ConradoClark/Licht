using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

namespace Licht.Unity.Effects
{
    public class ExpireByDistanceToObject : BaseGameAgent
    {
        [field: SerializeField]
        public PooledComponent Poolable { get; private set; }

        [field: SerializeField]
        public Transform Reference { get; private set; }

        [field: SerializeField]
        public Transform Target { get; private set; }

        [field: SerializeField]
        public float Distance { get; private set; }

        private bool _effectEnded;
        override protected void OnEnable()
        {
            _effectEnded = false;
            base.OnEnable();
        }

        override protected IEnumerable<IEnumerable<Action>> Handle()
        {
            if (!_effectEnded && !(Vector2.Distance(Reference.position, Target.position) < Distance))
            {
                Poolable.EndEffect();
                _effectEnded = true;
                yield break;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
