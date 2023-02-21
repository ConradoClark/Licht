using System.Collections.Generic;
using System.Linq;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

namespace Licht.Unity.Effects
{
    [DefaultExecutionOrder(-1)]
    public class EffectsManager : SceneObject<EffectsManager>
    {
        public ScriptPrefab[] PreloadEffects;
        public int DefaultPoolSize;
        public Dictionary<ScriptPrefab, PrefabPool> Effects { get; private set; }

        private void Awake()
        {
            Effects = new Dictionary<ScriptPrefab, PrefabPool>();
            foreach (var effect in PreloadEffects)
            {
                AddEffect(effect);
                if (effect.Pool.TryGetFromPool(out var component))
                {
                    effect.Pool.Release(component);
                }
            }
        }

        public void AddEffect(ScriptPrefab prefabRef)
        {
            var prefabPool = new GameObject($"EffectPool#[{prefabRef.Prefab.name}]")
            {
                transform =
                {
                    parent = transform
                }
            };

            var pool = prefabPool.AddComponent<PrefabPool>();
            pool.Size = prefabRef.DefaultPoolSize;
            pool.Prefab = prefabRef.Prefab;

            pool.Activate();

            Effects[prefabRef] = pool;
        }

        public PrefabPool GetEffect(ScriptPrefab prefabRef)
        {
            if (!Effects.ContainsKey(prefabRef))
            {
                AddEffect(prefabRef);
            }

            return Effects[prefabRef];
        }

        public PrefabPool GetEffect(GameObject prefab)
        {
            var scriptPrefab = Effects.Keys.FirstOrDefault(effect => effect.Prefab == prefab);
            if (scriptPrefab == null)
            {
                 scriptPrefab = ScriptableObject.CreateInstance<ScriptPrefab>();
                 scriptPrefab.name = prefab.name;
                 scriptPrefab.DefaultPoolSize = DefaultPoolSize;
            }

            return GetEffect(scriptPrefab);
        }
    }
}
