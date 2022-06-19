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
                var effectsManager = EffectsManager.Instance();
                if (effectsManager == null) return;

                effectsManager.AddEffect(this);
            }
        }

        public PrefabPool Pool => EffectsManager.Instance().GetEffect(this);
    }
}
