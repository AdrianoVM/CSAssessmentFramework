using System;
using System.Linq;
using Setup;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Options
{
    [CustomEditor(typeof(Manager), true)]
    public class ManagerEditor : Editor
    {

        private static class Styles
        {
            public static GUIContent AccelerationSettings(string title)
            {
                return EditorGUIUtility.TrTextContent(title, "Toggle to enable or disable changing the settings.");
            } 
            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var myManager = (Manager) target;
            //EditorGUILayout.PropertyField(m_name, new GUIContent("name"));
            //SerializedProperty globalInfoSP = serializedObject.FindProperty(nameof(Manager.globalInfo));
            //globalInfoSP = (GlobalInfo)EditorGUILayout.ObjectField("Global Info", myManager.globalInfo, typeof(GlobalInfo), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Manager.globalInfo)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Manager.managerName)));
            
            FileManager.FileButtons(myManager.globalInfo, myManager);

            SetupUtilities.DrawSeparatorLine();
            
            RenderInspectors(myManager);

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderInspectors(Manager myManager)
        {
            //TODO: Add a way to add missing MonoBehaviour
            var toRemove = -1;
            for (var i = 0; i < myManager.options.Count; i++)
            {
                InspectorOption option = myManager.options[i];
                GUILayout.Label(option.monoName);
                if (option.Mono == null)
                {
                    if (option.monoName != new InspectorOption().monoName)
                    {
                        Debug.Log("missed");
                        EditorGUILayout.HelpBox("Missing MonoBehaviour of type " + option.MonoType, MessageType.Warning);
                    }

                    myManager.options[i].Mono = (MonoBehaviour)EditorGUILayout.ObjectField(option.monoName,
                        myManager.options[i].Mono, typeof(MonoBehaviour), true);
                    if (GUILayout.Button("Remove"))
                    {
                        toRemove = i;
                    }
                }
                else
                {
                    switch (option.Mono)
                    {
                        case Manager:
                            option.Mono = null;
                            Debug.LogWarning("Do not try Infinite Recursion");
                            continue;
                    }
                    // if (option.Mono is TestManager )
                    // {
                    //     option.Mono = null;
                    //     continue;
                    // }

                    bool accelerationSettingToggled = option.enableOption;
                    GUILayout.BeginHorizontal();
                    option.expandOption = SetupUtilities.DrawToggleHeaderFoldout(Styles.AccelerationSettings(option.Mono.name),
                        option.expandOption, ref accelerationSettingToggled, 0f);
                    if (GUILayout.Button("Remove"))
                    {
                        toRemove = i;
                    }

                    GUILayout.EndHorizontal();
                    ++EditorGUI.indentLevel;
                    if (option.expandOption)
                    {
                        EditorGUI.BeginDisabledGroup(!option.enableOption);
                        if (option.Mono != null)
                        {
                            Editor testEditor = CreateEditor(option.Mono);
                            //testEditor.DrawDefaultInspector();
                            testEditor.OnInspectorGUI();
                        }

                        EditorGUI.EndDisabledGroup();
                    }

                    --EditorGUI.indentLevel;
                    EditorGUILayout.Space();
                    option.enableOption = accelerationSettingToggled;
                }
            }

            if (toRemove >= 0)
            {
                myManager.options.RemoveAt(toRemove);
            }

            if (!myManager.options.Any() || myManager.options.Last().Mono != null)
            {
                if (GUILayout.Button("Add Option"))
                {
                    myManager.options.Add(new InspectorOption());
                }
            }
        }
    }
}