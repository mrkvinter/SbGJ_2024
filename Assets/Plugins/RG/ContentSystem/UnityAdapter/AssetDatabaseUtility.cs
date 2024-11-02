#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RG.ContentSystem.UnityAdapter
{
    public static class AssetDatabaseUtility
    {
        public static T[] GetAllAssetsOfType<T>() where T : Object
        {
            var paths = AssetDatabase.FindAssets($"t:{typeof(T)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();

            var assets = new T[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(paths[i]);
            }

            return assets;
        }
    }
}
#endif