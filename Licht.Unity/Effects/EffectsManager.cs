using System;
using System.Collections.Generic;
using System.Text;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;
using UnityEngine.Windows.WebCam;

namespace Licht.Unity.Effects
{
    [DefaultExecutionOrder(-1)]
    public class EffectsManager : SceneObject<EffectsManager>
    {
        public Dictionary<ScriptPrefab, PrefabPool> Effects { get; private set; }

        private void Awake()
        {
            Effects = new Dictionary<ScriptPrefab, PrefabPool>();
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
    }
}
