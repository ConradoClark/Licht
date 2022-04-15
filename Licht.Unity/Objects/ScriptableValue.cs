using UnityEngine;

namespace Licht.Unity.Objects
{
    public abstract class ScriptableValue : ScriptableObject
    {
        public abstract object Value { get; }
    }
}