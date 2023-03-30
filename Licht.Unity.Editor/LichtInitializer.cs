using Licht.Unity.Objects;
using UnityEditor;
using UnityEngine;

namespace Licht.Unity.Editor
{
    public static class LichtInitializer
    {
        [MenuItem("Assets/Licht/Initialization/CreateScriptables", false, -10)]
        public static void CreateScriptables()
        {
            var path = GetCurrentPath();

            CreateScriptable<ScriptBasicMachinery>(path, "MainMachinery");
            CreateScriptable<ScriptDefaultTimer>(path, "GameTimer");
            var last = CreateScriptable<ScriptDefaultTimer>(path, "UITimer");

            Selection.activeObject = last;
        }

        private static string GetCurrentPath()
        {
            var selectedPath = "Assets";
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            if (selection.Length <= 0) return selectedPath;
            selectedPath = AssetDatabase.GetAssetPath(selection[0]);
            if (!AssetDatabase.IsValidFolder(selectedPath))
            {
                selectedPath = System.IO.Path.GetDirectoryName(selectedPath) ?? selectedPath;
            }

            return selectedPath;
        }

        private static T CreateScriptable<T>(string path, string name) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/{name}.asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            return asset;
        }
    }
}
