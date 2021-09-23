using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WhiteWillow
{
    public static class Utility
    {
        public static List<T> GetAllAssetsOfType<T>() where T : ScriptableObject
        {
            List<T> assets = new List<T>();

#if UNITY_EDITOR

            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                    assets.Add(asset);
            }

#endif

            return assets;
        }
    }
}
