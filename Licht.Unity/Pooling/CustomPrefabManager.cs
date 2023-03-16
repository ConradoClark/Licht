using System.Collections.Generic;
using Licht.Interfaces.Pooling;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Pooling
{
    [DefaultExecutionOrder(-1)]
    public class CustomPrefabManager<TPool, TPoolable> : MonoBehaviour 
        where TPool : GenericPrefabPool<TPoolable>
        where TPoolable : IPoolableComponent
    {
        public ScriptPrefab[] PreloadEffects;
        public string Name;
        public Dictionary<ScriptPrefab, TPool> Effects { get; private set; }

        private void Awake()
        {
            Effects = new Dictionary<ScriptPrefab, TPool>();
            foreach (var effect in PreloadEffects)
            {
                AddEffect(effect);
                if (Effects[effect].TryGetFromPool(out var component))
                {
                    Effects[effect].Release(component);
                }
            }
        }

        public void AddEffect(ScriptPrefab prefabRef)
        {
            var prefabPool = new GameObject($"{Name}#[{prefabRef.Prefab.name}]")
            {
                transform =
                {
                    parent = transform
                }
            };

            var pool = prefabPool.AddComponent<TPool>();
            pool.Size = prefabRef.DefaultPoolSize;
            pool.Prefab = prefabRef.Prefab;

            pool.Activate();

            Effects[prefabRef] = pool;
        }

        public TPool GetEffect(ScriptPrefab prefabRef)
        {
            if (!Effects.ContainsKey(prefabRef))
            {
                AddEffect(prefabRef);
            }

            return Effects[prefabRef];
        }
    }
}
