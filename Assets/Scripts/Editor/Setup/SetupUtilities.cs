using UnityEditor;
using UnityEngine;

namespace Setup
{
    /// <summary>
    /// Utilities Used by all the editor classes 
    /// </summary>
    public class SetupUtilities
    {
        public static void DrawSeparatorLine(int width = 4)
        {
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(12));
            rect.height = 1;
            rect.y = rect.y + 5;
            rect.x = width;
            rect.width -= width;
            EditorGUI.DrawRect(rect, new Color(0.35f, 0.35f, 0.35f));
        }
        
        
        /// <summary>
        /// Taken From Unity's Terrain Tool GUI Helper
        /// </summary>
        public static void DrawToggleHeaderFoldout(GUIContent title, ref bool state, ref bool enabled,float padding)
        {
            Rect backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            Rect labelRect = backgroundRect;
            labelRect.xMin += 32f;
            labelRect.xMax -= 20f;

            Rect foldoutRect = backgroundRect;
            foldoutRect.xMin += padding;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            Rect toggleRect = foldoutRect;
            toggleRect.x = foldoutRect.xMax + 4f;

            Rect selectRect = toggleRect;
            selectRect.x = labelRect.xMax + 4f;

            // Background rect should be full-width
            backgroundRect.xMin = padding;
            backgroundRect.xMin = 0;

            backgroundRect.width += 4f;

            // Background
            var backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

            // Title
            EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

            // Active checkbox
            state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

            // Enabled toggle
            enabled = GUI.Toggle(toggleRect, enabled, GUIContent.none, EditorStyles.toggle);
            
            Event e = Event.current;

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
        }
    }
}