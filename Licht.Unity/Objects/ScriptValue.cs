using UnityEngine;

namespace Licht.Unity.Objects
{
    public abstract class ScriptValue : ScriptableObject
    {
        public abstract object Value { get; }
    }
}