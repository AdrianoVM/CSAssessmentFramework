using System.Collections.Generic;
using System.Linq;
using Setup.Data;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Setup
{
    public class MovementWindow
    {
        public List<ShowableOption> MovementOptions = new(Enumerable.Repeat<ShowableOption>(new ShowableOption(), 1));
        private SetupSettings _Settings = ScriptableObject.CreateInstance<SetupSettings>();
        static class Styles
        {
            public static GUIContent AccelerationSettings(string title)
            {
                return EditorGUIUtility.TrTextContent(title, "Toggle to enable or disable changing the settings.");
            } 
            
        }

        
        public void OnGUI()
        {
            //MovementOptions[0] = (MonoBehaviour) EditorGUILayout.ObjectField("XR Origin",MovementOptions[0], typeof(MonoBehaviour), true);
            //MovementOptions[1] = (MonoBehaviour) EditorGUILayout.ObjectField("XR Origin",MovementOptions[1], typeof(MonoBehaviour), true);
            int toRemove = -1;
            for (int i = 0; i < MovementOptions.Count; i++)
            {
                ShowableOption option = MovementOptions[i];
                if (option.MonoBehaviour == null)
                {
                    MovementOptions[i].MonoBehaviour = (MonoBehaviour) EditorGUILayout.ObjectField("New Option ",MovementOptions[i].MonoBehaviour, typeof(MonoBehaviour), true);
                }
                else
                {

                    bool accelerationSettingToggled = option.EnableOption;
                    GUILayout.BeginHorizontal();
                    option.ExpandOption = SetupUtilities.DrawToggleHeaderFoldout(Styles.AccelerationSettings(option.MonoBehaviour.name), option.ExpandOption, ref accelerationSettingToggled, 0f);
                    if (GUILayout.Button("Remove"))
                    {
                        toRemove = i;
                    }
                    GUILayout.EndHorizontal();
                    ++EditorGUI.indentLevel;
                    if (option.ExpandOption)
                    {
                        EditorGUI.BeginDisabledGroup(!option.EnableOption);
                        ShowAccelerationSettings(option.MonoBehaviour);
                        EditorGUI.EndDisabledGroup();
                    }
                    --EditorGUI.indentLevel;
                    EditorGUILayout.Space ();
                    option.EnableOption = accelerationSettingToggled;
                    
                }
                
            }

            if (toRemove >= 0)
            {
                MovementOptions.RemoveAt(toRemove);
            }
            if (!MovementOptions.Any() || MovementOptions.Last().MonoBehaviour != null)
            {
                if (GUILayout.Button("Add Option"))
                {
                    MovementOptions.Add(new ShowableOption());
                }
            }

        }

        private void ShowAccelerationSettings(MonoBehaviour option)
        {
            EditorGUILayout.LabelField("Level", "banana");
            //MovementOptions= EditorGUILayout.ObjectField("XR Origin",MovementOptions, typeof(MonoBehaviour), true);
            if (MovementOptions != null)
            {
                Editor testEditor = Editor.CreateEditor(option);
                //testEditor.DrawDefaultInspector();
                testEditor.OnInspectorGUI();
            }
        }

        
    }
}