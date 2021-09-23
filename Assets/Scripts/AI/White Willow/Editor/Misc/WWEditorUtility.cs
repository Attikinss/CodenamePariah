using UnityEditor;
using UnityEngine;

namespace WhiteWillow.Editor
{
    public static class WWEditorUtility
    {
        public static void DrawLineSeparator(float thickness = 1.0f)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, thickness);
            rect.height = thickness;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            EditorGUILayout.Space();
        }
    }
}
