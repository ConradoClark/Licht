using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.Juicers.JuiceExtensions.Transform
{
    [AddComponentMenu("L. Juicers: Uniform Scale")]
    public class UniformScale : BaseGameRunner
    {
        [field:CustomLabel("A sprite Transformer is required to use this juicer.")]
        [field: SerializeField]
        public Transformer SpriteTransformer { get; private set; }

        [field:BeginFoldout("Parameters")]
        [field: CustomLabel("Select this if an initial value should be forced before scaling.")]
        [field: SerializeField]
        public bool ShouldSetInitialValue { get; private set; }

        [field:ShowWhen(nameof(ShouldSetInitialValue))]
        [field: SerializeField]
        public Vector2 InitialScale { get; private set; }

        [field: SerializeField]
        public Vector2 TargetScale { get; private set; }

        [field: SerializeField]
        public float TimeInSeconds { get; private set; }

        [field: SerializeField]
        public EasingYields.EasingFunction Easing { get; private set; }

        protected override IEnumerable<IEnumerable<Action>> Handle()
        {
            if (ShouldSetInitialValue) SpriteTransformer.Target.transform.localScale = InitialScale;

            yield return new LerpBuilder()
                .SetTarget(1f)
                .Easing(Easing)
                .Over(TimeInSeconds)
                .UsingTimer(Timer)
                .OnEachStep(f=> SpriteTransformer.ApplyScale(Vector3.Lerp(InitialScale, TargetScale, f)))
                .Build();
        }
    }
}
