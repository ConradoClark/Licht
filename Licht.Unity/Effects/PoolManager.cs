using System.Collections.Generic;
using System.Linq;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.Effects
{
    [AddComponentMenu("L!> Pooling: Pool Manager")]
    [DefaultExecutionOrder(-1)]
    public class PoolManager : SceneObject<PoolManager>
    {
        [field:CustomLabel("Object Pools that should be instantiated at start.")]
        public ScriptPrefab[] PreLoadObjects;
        [field:CustomLabel("Default pool size when not specified by the Script Prefab.")]
        public int DefaultPoolSize;
        public Dictionary<ScriptPrefab, PrefabPool> PooledObjects { get; private set; }

        private void Awake()
        {
            PooledObjects = new Dictionary<ScriptPrefab, PrefabPool>();
            foreach (var obj in PreLoadObjects)
            {
                AddPool(obj);
                if (obj.Pool.TryGetFromPool(out var component))
                {
                    obj.Pool.Release(component);
                }
            }
        }

        public void AddPool(ScriptPrefab prefabRef)
        {
            var prefabPool = new GameObject($"ObjectPool#[{prefabRef.Prefab.name}]")
            {
                transform =
                {
                    parent = transform
                }
            };

            var pool = prefabPool.AddComponent<PrefabPool>();
            pool.Size = prefabRef.DefaultPoolSize;
            pool.Prefab = prefabRef.Prefab;
            prefabRef.Pool = pool;
            pool.Activate();

            PooledObjects[prefabRef] = pool;
        }

        public PrefabPool GetPool(ScriptPrefab prefabRef)
        {
            if (!PooledObjects.ContainsKey(prefabRef))
            {
                AddPool(prefabRef);
            }

            return PooledObjects[prefabRef];
        }

        public PrefabPool GetPool(GameObject prefab)
        {
            var scriptPrefab = PooledObjects.Keys.FirstOrDefault(effect => effect.Prefab == prefab);
            if (scriptPrefab == null)
            {
                 scriptPrefab = ScriptableObject.CreateInstance<ScriptPrefab>();
                 scriptPrefab.name = prefab.name;
                 scriptPrefab.DefaultPoolSize = DefaultPoolSize;
            }

            return GetPool(scriptPrefab);
        }
    }
}
