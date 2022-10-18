using System.Linq;
using Options;
using Setup;
using UnityEditor;
using UnityEngine;

namespace Learning
{
    [CustomEditor(typeof(TestManager))]
    public class TestManagerEditor : Editor
    {
        static class Styles
        {
            public static GUIContent AccelerationSettings(string title)
            {
                return EditorGUIUtility.TrTextContent(title, "Toggle to enable or disable changing the settings.");
            } 
            
        }
        
        public override void OnInspectorGUI()
        {
            var myTestManager = (TestManager) target;
            if (GUILayout.Button("Load"))
            {
                myTestManager.OnLoad();
            }
            if (GUILayout.Button("Save"))
            {
                myTestManager.OnSave();
            }
            if (GUILayout.Button("Load JSON"))
            {
                myTestManager.OnJsonLoad();
            }
            if (GUILayout.Button("Save JSON"))
            {
                myTestManager.OnJsonSave();
            }
            if (GUILayout.Button("Load test"))
            {
                myTestManager.testMono.OnLoad();
            }
            if (GUILayout.Button("Save test"))
            {
                myTestManager.testMono.OnSave();
            }
            base.OnInspectorGUI();

            SetupUtilities.DrawSeparatorLine();

            
            //TODO: Add a way to add missing monobehaviour
            int toRemove = -1;
            for (int i = 0; i < myTestManager.movementOptions.Count; i++)
            {
                ShowableOption option = myTestManager.movementOptions[i];
                GUILayout.Label(option.monoName);
                if (option.Mono == null)
                {
                    if (option.monoName != new ShowableOption().monoName)
                    {
                        Debug.Log("missed");
                        EditorGUILayout.HelpBox("Missing MonoBehaviour of type " + option.MonoType, MessageType.Warning);
                    }
                    myTestManager.movementOptions[i].Mono = (MonoBehaviour) EditorGUILayout.ObjectField(option.monoName,myTestManager.movementOptions[i].Mono, typeof(MonoBehaviour), true);
                    if (GUILayout.Button("Remove"))
                    {
                        toRemove = i;
                    }
                }
                else
                {
                    switch (option.Mono)
                    {
                        case TestManager : 
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
                    option.expandOption = SetupUtilities.DrawToggleHeaderFoldout(Styles.AccelerationSettings(option.Mono.name), option.expandOption, ref accelerationSettingToggled, 0f);
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
                    EditorGUILayout.Space ();
                    option.enableOption = accelerationSettingToggled;
                    
                }
                
            }

            if (toRemove >= 0)
            {
                myTestManager.movementOptions.RemoveAt(toRemove);
            }
            if (!myTestManager.movementOptions.Any() || myTestManager.movementOptions.Last().Mono != null)
            {
                if (GUILayout.Button("Add Option"))
                {
                    myTestManager.movementOptions.Add(new ShowableOption());
                }
            }
        }
    }
}