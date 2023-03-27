using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

namespace Licht.Unity.Effects
{
    public class ExpireAfterAnimation : BaseGameRunner
    {
        [field: SerializeField]
        public EffectPoolable Poolable { get; private set; }

        [field: SerializeField]
        public Animator Animator { get; private set; }
        [field:SerializeField]
        public string State { get; private set; }

        override protected void OnEnable()
        {
            base.OnEnable();
            Animator.Play(State);
        }

        override protected IEnumerable<IEnumerable<Action>> Handle()
        {
            yield return TimeYields.WaitOneFrameX;

            while (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1 || Animator.IsInTransition(0))
            {
                yield return TimeYields.WaitOneFrameX;
            }

            Poolable.EndEffect();
        }
    }
}