using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Licht.Unity.Objects;
using UnityEditor;
using UnityEngine;

namespace Licht.Unity.Editor
{
    public static class ToolboxContextMenu
    {
        [MenuItem("GameObject/Licht/Toolbox")]
        private static void CreateToolbox()
        {
            var selectedObject = Selection.activeGameObject;

            // Create a new GameObject
            var toolbox = new GameObject("Toolbox");

            if (selectedObject != null)
            {
                // If a GameObject is selected, make the new object a child of the selected object
                toolbox.transform.parent = selectedObject.transform;
            }

            // Add components to the new GameObject
            var basicToolbox = toolbox.AddComponent<BasicToolbox>();

            var defaultGameTimer = toolbox.GetComponent<DefaultGameTimer>();
            var defaultUITimer = toolbox.GetComponent<DefaultUITimer>();
            var defaultMachinery = toolbox.GetComponent<DefaultMachinery>();

            var objects = new List<ScriptValue>();

            if (TryGetScriptable<ScriptDefaultTimer>("GameTimer", out var gameTimer))
            {
                defaultGameTimer.TimerRef = gameTimer;
                if (gameTimer != null) objects.Add(gameTimer);
            }

            if (TryGetScriptable<ScriptDefaultTimer>("UITimer", out var uiTimer))
            {
                defaultUITimer.TimerRef = uiTimer;
                if (uiTimer != null) objects.Add(uiTimer);
            }

            if (TryGetScriptable<ScriptBasicMachinery>("MainMachinery", out var mainMachinery))
            {
                defaultMachinery.MachineryRef = mainMachinery;
                if (mainMachinery != null) objects.Add(mainMachinery);
            }

            basicToolbox.ScriptableObjects = objects;

            // Set the new GameObject as the selected object in the Hierarchy window
            Selection.activeGameObject = toolbox;
        }

        private static bool TryGetScriptable<T>(string name, out T? obj) where T : ScriptableObject
        {
            var type = $"t:{typeof(T).Name}"; // The type of ScriptableObject to find
            var assetGuid = AssetDatabase.FindAssets($"{type} {name}").FirstOrDefault() ?? string.Empty;
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));

            if (asset != null && asset.name == name && asset is T result)
            {
                obj = result;
                return true;
            }

            obj = null;
            return false;
        }
    }
}
