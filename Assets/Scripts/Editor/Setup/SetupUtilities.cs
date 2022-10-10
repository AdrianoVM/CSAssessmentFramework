using UnityEditor;
using UnityEngine;

namespace Setup
{
    /// <summary>
    /// Utilities Used by all the editor classes 
    /// </summary>
    public class SetupUtilities
    {
        public static void DrawSeparatorLine()
        {
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(12));
            rect.height = 1;
            rect.y = rect.y + 5;
            rect.x = 2;
            rect.width -= 4;
            EditorGUI.DrawRect(rect, new Color(0.35f, 0.35f, 0.35f));
        }
        
        
        /// <summary>
        /// Taken From Unity's Terrain Tool GUI Helper
        /// </summary>
        /// <param name="title"></param>
        /// <param name="state"></param>
        /// <param name="enabled"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static bool DrawToggleHeaderFoldout(GUIContent title, bool state, ref bool enabled, float padding)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 32f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.xMin += padding;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            var toggleRect = foldoutRect;
            toggleRect.x = foldoutRect.xMax + 4f;

            // Background rect should be full-width
            backgroundRect.xMin = padding;
            backgroundRect.xMin = 0;

            backgroundRect.width += 4f;

            // Background
            float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

            // Title
            EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

            // Active checkbox
            state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

            // Enabled toggle
            enabled = GUI.Toggle(toggleRect, enabled, GUIContent.none, EditorStyles.toggle);

            var e = Event.current;

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                if (toggleRect.Contains(e.mousePosition))
                {
                    enabled = !enabled;
                    e.Use();
                }
                else if (backgroundRect.Contains(e.mousePosition))
                {
                    state = !state;
                    e.Use();
                }
            }

            return state;
        }
    }
}