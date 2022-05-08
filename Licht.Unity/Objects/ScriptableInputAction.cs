using UnityEngine;

namespace Licht.Unity.Objects
{
    [CreateAssetMenu(fileName = "InputAction", menuName = "Licht/Input/InputAction", order = 1)]
    public class ScriptableInputAction : ScriptableObject
    {
        public string ActionName;
    }
}
