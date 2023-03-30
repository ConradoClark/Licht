using System;
using System.Collections.Generic;
using System.Text;
using Licht.Unity.Objects;
using UnityEditor;
using UnityEngine;
using PrefabType = Licht.Unity.Objects.PrefabType;

namespace Licht.Unity.Editor.Pooling
{
    public static class PrefabToScriptableObject
    {
        [MenuItem("Assets/Licht/Prefab/CreateEffectPrefab",false, 0)]
        public static void CreateEffectPrefab(MenuCommand command)
        {
            CreateScriptableObject(command, PrefabType.Effect);
        }

        [MenuItem("Assets/Licht/Prefab/CreateDefaultPrefab", false, 1)]
        public static void CreateDefaultPrefab(MenuCommand command)
        {
            CreateScriptableObject(command, PrefabType.Default);
        }

        private static void CreateScriptableObject(MenuCommand command, PrefabType type)
        {
            // Get the selected Prefab
            var prefab = command.context as GameObject;

            if (prefab == null)
            {
                Debug.LogWarning("Select a prefab first!");
                return;
            }

            // Create a new ScriptableObject
            var scriptableObject = ScriptableObject.CreateInstance<ScriptPrefab>();

            // Set the ScriptableObject's properties based on the selected Prefab
            scriptableObject.Prefab = prefab;
            scriptableObject.Type = type;
            scriptableObject.DefaultPoolSize = 30;

            // Save the ScriptableObject to a new asset file
            var assetPath = AssetDatabase.GetAssetPath(prefab);
            var assetName = $"{prefab.name}_POOL.asset";
            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(assetPath) ?? string.Empty, assetName));
            AssetDatabase.CreateAsset(scriptableObject, assetPathAndName);
            AssetDatabase.SaveAssets();

            // Select the new asset in the Project window
            Selection.activeObject = scriptableObject;
        }
    }
}
