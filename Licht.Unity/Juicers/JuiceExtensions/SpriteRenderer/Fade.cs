using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Juicers.JuiceExtensions.SpriteRenderer
{
    [AddComponentMenu("JUICE_Fade")]
    public class Fade : BaseGameRunner
    {
        [field:SerializeField]
        public UnityEngine.SpriteRenderer SpriteRenderer { get; private set; }

        [field: SerializeField]
        public float InitialAlpha { get; private set; }

        [field: SerializeField]
        public float TargetAlpha { get; private set; }

        [field: SerializeField]
        public float TimeInSeconds { get; private set; }

        [field: SerializeField]
        public EasingYields.EasingFunction EasingFunction { get; private set; } 

        protected override IEnumerable<IEnumerable<Action>> Handle()
        {
            yield return SpriteRenderer.GetAccessor()
                .Color.A
                .SetTarget(TargetAlpha)
                .Over(TimeInSeconds)
                .Easing(EasingFunction)
                .UsingTimer(Timer)
                .OnStart(() =>
                {
                    SpriteRenderer.color = new Color(SpriteRenderer.color.r,
                        SpriteRenderer.color.g,
                        SpriteRenderer.color.b,
                        InitialAlpha);
                })
                .Build();
        }
    }
}
