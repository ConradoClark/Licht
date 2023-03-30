using System;
using System.Collections.Generic;
using System.Text;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

namespace Licht.Unity.Effects
{
    public class ExpireByDistanceToPoint : BaseGameObject
    {
        [field: SerializeField]
        public PooledComponent Poolable { get; private set; }

        [field: SerializeField]
        public Transform Reference { get; private set; }

        [field: SerializeField]
        public Vector3 Target { get; private set; }

        [field: SerializeField]
        public float Distance { get; private set; }

        private bool _effectEnded;
        override protected void OnEnable()
        {
            _effectEnded = false;
            base.OnEnable();
        }
        private void Update()
        {
            if (_effectEnded || Vector2.Distance(Reference.position, Target) < Distance)
            {
                return;
            }

            Poolable.EndEffect();
            _effectEnded = true;
        }

    }
}
