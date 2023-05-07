using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public static class SpriteRendererJuiceExtensions
{
    public static LerpBuilder FadeIn(this SpriteRenderer spriteRenderer,
        float timeInSeconds = 1f,
        float initialAlpha = 0f,
        float targetAlpha = 1f)
    {   
        return spriteRenderer.GetAccessor()
            .Color.A
            .SetTarget(targetAlpha)
            .Over(timeInSeconds)
            .OnStart(() =>
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r,
                    spriteRenderer.color.g,
                    spriteRenderer.color.b,
                    initialAlpha);
            });
    }

    public static LerpBuilder FadeOut(this SpriteRenderer spriteRenderer,
        float timeInSeconds = 1f,
        float initialAlpha = 1f,
        float targetAlpha = 0f)
    {
        return FadeIn(spriteRenderer, timeInSeconds, initialAlpha, targetAlpha);
    }

    public static IEnumerable<IEnumerable<Action>> BlinkForSeconds(this SpriteRenderer spriteRenderer,
        float timeInSeconds, float blinkFrequencyInMs = 100, ITimer timer = null)
    {
        var usableTimer = timer ?? SceneObject<DefaultGameTimer>.Instance(true).TimerRef.Timer;
        var wait = TimeYields.WaitSeconds(usableTimer, timeInSeconds);

        yield return wait.UntilAny(TimeYields.WaitMilliseconds(usableTimer, blinkFrequencyInMs)
            .ThenRun(() => spriteRenderer.enabled = !spriteRenderer.enabled).Infinite());

        spriteRenderer.enabled = true;

    }
} 