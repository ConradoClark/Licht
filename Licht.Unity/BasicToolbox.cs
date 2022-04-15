using System.Collections.Generic;
using System.Linq;
using Licht.Interfaces.Update;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity
{
    public class BasicToolbox : MonoBehaviour
    {
        public List<ScriptableValue> ScriptableObjects;
        private IUpdateable[] _updateableScriptableObjects;

        protected void Awake()
        {
            _updateableScriptableObjects = ScriptableObjects.Select(obj => obj.Value).OfType<IUpdateable>().ToArray();
        }

        protected void Update()
        {
            foreach (var scriptableObject in _updateableScriptableObjects)
            {
                scriptableObject.Update();
            }
        }
    }
}
