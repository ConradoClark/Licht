using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Juicers.JuiceExtensions.SpriteRenderer
{
    [AddComponentMenu("JUICE_FadeShaderPropertyInt")]
    public class FadeShaderPropertyInt : BaseGameRunner
    {
        [field: SerializeField]
        public UnityEngine.SpriteRenderer SpriteRenderer { get; private set; }

        [field: SerializeField]
        public string PropertyName { get; private set; }
        [field: SerializeField]
        public bool ShouldSetInitialValue { get; private set; }

        [field: SerializeField]
        public int InitialValue { get; private set; }

        [field: SerializeField]
        public int TargetValue { get; private set; }

        [field: SerializeField]
        public float TimeInSeconds { get; private set; }

        [field: SerializeField]
        public EasingYields.EasingFunction EasingFunction { get; private set; }

        protected override IEnumerable<IEnumerable<Action>> Handle()
        {
            yield return SpriteRenderer.GetAccessor()
                .Material(PropertyName)
                .AsInt()
                .SetTarget(TargetValue)
                .Over(TimeInSeconds)
                .Easing(EasingFunction)
                .UsingTimer(Timer)
                .OnStart(() =>
                {
                    if (ShouldSetInitialValue) SpriteRenderer.material.SetInt(PropertyName, InitialValue);
                })
                .Build();
        }
    }
}
