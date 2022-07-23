using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Pooling;
using UnityEngine;

namespace Licht.Unity.Effects
{
    public class OneShotAnimationEffect : EffectPoolable
    {
        public Animator Animator;
        public string State;

        public override void OnActivation()
        {
            Animator.Play(State);
            DefaultMachinery.AddBasicMachine(WaitUntilAnimationIsFinished());
        }

        private IEnumerable<IEnumerable<Action>> WaitUntilAnimationIsFinished()
        {
            yield return TimeYields.WaitOneFrameX;

            while (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1 || Animator.IsInTransition(0))
            {
                yield return TimeYields.WaitOneFrameX;
            }

            EndEffect();
        }
    }
}