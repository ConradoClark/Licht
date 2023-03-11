using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Juicers.JuiceExtensions.Transform
{
    [AddComponentMenu("JUICE_UniformScale")]
    public class UniformScale : BaseGameRunner
    {
        [field: SerializeField]
        public SpriteTransformer SpriteTransformer { get; private set; }

        [field: SerializeField]
        public bool ShouldSetInitialValue { get; private set; }

        [field: SerializeField]
        public Vector2 InitialScale { get; private set; }

        [field: SerializeField]
        public Vector2 TargetScale { get; private set; }

        [field: SerializeField]
        public float TimeInSeconds { get; private set; }

        [field: SerializeField]
        public EasingYields.EasingFunction EaseIn { get; private set; }

        [field: SerializeField]
        public EasingYields.EasingFunction EaseOut { get; private set; }

        protected override IEnumerable<IEnumerable<Action>> Handle()
        {
            if (ShouldSetInitialValue) SpriteTransformer.SpriteRenderer.transform.localScale = InitialScale;

            yield return new LerpBuilder()
                .SetTarget(1f)
                .Easing(EaseIn)
                .Over(TimeInSeconds*0.5f)
                .UsingTimer(Timer)
                .OnEachStep(f=> SpriteTransformer.ApplyScale(Vector3.Lerp(InitialScale, TargetScale, f)))
                .Build();

            yield return new LerpBuilder(0f)
                .SetTarget(1f)
                .Easing(EaseOut)
                .Over(TimeInSeconds * 0.5f)
                .UsingTimer(Timer)
                .OnEachStep(f => SpriteTransformer.ApplyScale(Vector3.Lerp(TargetScale, InitialScale, f)))
                .Build();
        }
    }
}
