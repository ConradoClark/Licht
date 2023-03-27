using System;
using System.Collections.Generic;
using System.Text;
using Licht.Unity.Effects;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

namespace Licht.Unity.Extensions
{
    public static class ScriptPrefabExtensions
    {
        public static bool TrySpawnEffect(this ScriptPrefab prefab, Vector3 position, out IPoolableComponent effect)
        {
            var result = SceneObject<EffectsManager>.Instance(true)
                .GetEffect(prefab)
                .TryGetFromPool(out effect);

            if (result)
            {
                effect.Component.transform.position = position;
            }

            return result;
        }

        public static bool TrySpawnEffect(this ScriptPrefab prefab, Vector3 position, Action<IPoolableComponent> init, out IPoolableComponent effect)
        {
            var result = SceneObject<EffectsManager>.Instance(true)
                .GetEffect(prefab)
                .TryGetFromPool(out effect, init);

            if (result)
            {
                effect.Component.transform.position = position;
            }

            return result;
        }
    }
}
