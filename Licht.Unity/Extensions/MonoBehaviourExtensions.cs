using System.Linq;
using Licht.Unity.Physics;
using Object = UnityEngine.Object;

namespace Licht.Unity.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static LichtPhysics GetLichtPhysics(this Object _)
        {
            return BasicToolbox.Instance().ScriptableObjects.OfType<LichtPhysics>().FirstOrDefault();
        }

    }
}
