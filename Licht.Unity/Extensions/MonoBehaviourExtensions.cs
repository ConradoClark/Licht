using System.Linq;
using Licht.Unity.Effects;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Pooling;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Licht.Unity.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static LichtPhysics GetLichtPhysics(this Object _)
        {
            return BasicToolbox.Instance().ScriptableObjects.OfType<LichtPhysics>().FirstOrDefault();
        }

        public static T FromScene<T>(this T obj, bool includeInactive = false) where T : MonoBehaviour
        {
            return SceneObject<T>.Instance(includeInactive);
        }

        public static PrefabPool GetPoolFromEffects(this MonoBehaviour obj, ScriptPrefab prefab)
        {
            return SceneObject<EffectsManager>.Instance(true).GetEffect(prefab);
        }

    }
}
