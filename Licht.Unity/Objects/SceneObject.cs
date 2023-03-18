using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Licht.Unity.Objects
{
    public class SceneObject<T> : MonoBehaviour where T : Object
    {
        private static T _storedObject;
        private static readonly Dictionary<string, T> NamedObjects = new Dictionary<string, T>();
        private static bool _notFound;
        static SceneObject()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private static void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _storedObject = null;
            _notFound = false;
            NamedObjects.Clear();
        }

        public static T FindOrCreate(string defaultName, bool includeInactive = false)
        {
            if (_storedObject == null)
            {
                _storedObject = FindObjectOfType<T>(includeInactive);
            }

            if (_storedObject == null && typeof(T).IsAssignableFrom(typeof(Component)))
            {
                var obj = new GameObject(defaultName);
                _storedObject = obj.AddComponent(typeof(T)) as T;
            }

            return _storedObject;
        }

        public static T Instance(bool includeInactive=false, bool includeNew = false)
        {
            if (_storedObject == null && (includeNew || !_notFound))
            {
                _storedObject = FindObjectOfType<T>(includeInactive);
                if (_storedObject == null)
                {
                    _notFound = true;
                }
            }

            return _storedObject;
        }

        public static T Instance(string gameObjectName)
        {
            if (!NamedObjects.ContainsKey(gameObjectName))
            {
                NamedObjects[gameObjectName] = FindObjectsOfType<T>().FirstOrDefault(obj => obj.name == gameObjectName);
            }

            return NamedObjects[gameObjectName];
        }
    }
}
