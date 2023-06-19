using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.Effects
{
    [AddComponentMenu("L. Pooling: Expire After Animation")]
    public class ExpireAfterAnimation : BaseGameAgent
    {
        [field: SerializeField]
        public Animator Animator { get; private set; }

        [field:CustomLabel("Object should expire after leaving the following Animator State.")]
        [field: SerializeField]
        public string AnimatorState { get; set; }

        [field: SerializeField]
        public int AnimationLayerIndex { get; set; } = 0;

        [field:CustomHeader("Conditions")]

        [field: SerializeField]
        public bool PlayAnimationOnEnable { get; set; }

        [field:CustomLabel("Select if object should expire as soon as it transitions to a different state.")]
        [field: SerializeField]
        public bool CheckTransition { get; set; }

        public PooledObject Poolable { get; private set; }

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
            base.OnEnable();
            if (PlayAnimationOnEnable)
            {
                Animator.Play(AnimatorState);
            }
        }

        override protected IEnumerable<IEnumerable<Action>> Handle()
        {
            yield return TimeYields.WaitOneFrameX;

            while (!Animator.GetCurrentAnimatorStateInfo(AnimationLayerIndex).IsName(AnimatorState))
            {
                yield return TimeYields.WaitOneFrameX;
            }

            while (Animator.GetCurrentAnimatorStateInfo(AnimationLayerIndex).normalizedTime < 1
                   || (CheckTransition && Animator.IsInTransition(AnimationLayerIndex)))
            {
                yield return TimeYields.WaitOneFrameX;
            }

            Poolable.Release();
        }
    }
}