﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Licht.Unity.Objects
{
    public class SceneObject<T> : MonoBehaviour where T : Object
    {
        private static T _storedObject;
        private static readonly Dictionary<string, T> NamedObjects = new Dictionary<string, T>();

        static SceneObject()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private static void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _storedObject = null;
            NamedObjects.Clear();
        }

        public static T Instance()
        {
            if (_storedObject == null)
            {
                _storedObject = FindObjectOfType<T>();
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