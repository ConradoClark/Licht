using Licht.Unity.Effects;
using Licht.Unity.Pooling;
using UnityEngine;

namespace Licht.Unity.Objects
{
    [CreateAssetMenu(fileName = "Prefab", menuName = "Licht/Prefabs/Prefab", order = 1)]
    public class ScriptPrefab : ScriptableObject
    {
        public GameObject Prefab;
        public int DefaultPoolSize;
        public PrefabType Type;

        private void Awake()
        {
            if (Type == PrefabType.Effect)
            {
                var effectsManager = PoolManager.Instance();
                if (effectsManager == null) return;

                effectsManager.AddPool(this);
            }
        }

        private PrefabPool _pool;
        public PrefabPool Pool
        {
            get
            {
                if (_pool == null && PoolManager.Instance() != null)
                {
                    _pool = PoolManager.Instance().GetPool(this);
                }

                return _pool;
            }
            set => _pool = value;
        }
    }
}
