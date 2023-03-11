using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Juicers.JuiceExtensions.SpriteRenderer
{
    [AddComponentMenu("JUICE_Blink")]
    public class Blink : BaseGameRunner
    {
        [field:SerializeField]
        public UnityEngine.SpriteRenderer SpriteRenderer { get; private set; }

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
