using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.Juicers.JuiceExtensions.SpriteRenderer
{
    [AddComponentMenu("L. Juicers: Sprite Blink")]
    public class Blink : BaseGameRunner
    {
        [field: CustomLabel("Blink target")]
        [field: SerializeField]
        public UnityEngine.SpriteRenderer SpriteRenderer { get; private set; }

        [field: BeginFoldout("Parameters")]
        [field: CustomLabel("Total Blink Duration, in seconds")]
        [field: SerializeField]
        public float TimeInSeconds { get; private set; }

        [field: SerializeField]
        public float BlinkFrequencyInMs { get; private set; }

        protected override IEnumerable<IEnumerable<Action>> Handle()
        {
            var wait = TimeYields.WaitSeconds(Timer, TimeInSeconds);

            yield return wait.UntilAny(TimeYields.WaitMilliseconds(Timer, BlinkFrequencyInMs)
                .ThenRun(() => SpriteRenderer.enabled = !SpriteRenderer.enabled).Infinite());

            SpriteRenderer.enabled = true;
        }
    }
}
