using System;
using System.Collections.Generic;
using System.Linq;
using Learning;
using Options;
using Setup.Data;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Setup
{
    [Serializable]
    public class MovementWindow
    {

        public List<InspectorOption> movementOptions = new(Enumerable.Repeat(new InspectorOption(), 1));
        //private SetupSettings _Settings = ScriptableObject.CreateInstance<SetupSettings>();
        
        [SerializeField]
        private List<InspectorOption> movementOptions2 = new(Enumerable.Repeat(new InspectorOption(), 1));
        [SerializeField]
        private int testInt;

        [SerializeField] private InspectorOption testOpt = new InspectorOption();

        private ScriptableObject _target;
        private SerializedObject so;
        private SerializedProperty movementOptionsProperty;
        private SerializedProperty testOptProperty;
        
        static class Styles
        {
            public static GUIContent AccelerationSettings(string title)
            {
                return EditorGUIUtility.TrTextContent(title, "Toggle to enable or disable changing the settings.");
            } 
            
        }

        // private void OnEnable()
        // {
        //     var data = EditorPrefs.GetString("MovementWindow", JsonUtility.ToJson(this, true));
        //     JsonUtility.FromJsonOverwrite(data, this);
        //     //_target = this;
        //     //so = new SerializedObject(_target);
        //     //movementOptionsProperty = so.FindProperty("movementOptions2");
        //     //testOptProperty = so.FindProperty("testOpt");
        //     Debug.Log(movementOptionsProperty);
        //     Debug.Log("pain");
        // }
        //
        // private void OnDisable()
        // {
        //     var data = JsonUtility.ToJson(this, true);
        //     EditorPrefs.SetString("MovementWindow", data);
        //     Debug.Log("End of pain");
        // }

        public void OnGUI2()
        {
            
            //MovementOptions[0] = (MonoBehaviour) EditorGUILayout.ObjectField("XR Origin",MovementOptions[0], typeof(MonoBehaviour), true);
            //MovementOptions[1] = (MonoBehaviour) EditorGUILayout.ObjectField("XR Origin",MovementOptions[1], typeof(MonoBehaviour), true);
            //so.Update();
            
            testInt = EditorGUILayout.IntField("exp",testInt);
            return;
            //Debug.Log(movementOptionsProperty.arraySize);

            int toRemove = -1;
            for (int i = 0; i < movementOptionsProperty.arraySize; i++)
            {
                SerializedProperty optionRef = movementOptionsProperty.GetArrayElementAtIndex(i);
                SerializedProperty enableOptionRef = optionRef.FindPropertyRelative(nameof(InspectorOption.enableOption));
                SerializedProperty expandOptionRef = optionRef.FindPropertyRelative(nameof(InspectorOption.expandOption));
                SerializedProperty monoRef = optionRef.FindPropertyRelative(nameof(InspectorOption.Mono));

                monoRef.objectReferenceValue = EditorGUILayout.ObjectField("My Custom Go", monoRef.objectReferenceValue, typeof(MonoBehaviour), true);
                
                continue;
                if (monoRef.objectReferenceValue == null)
                {
                    movementOptions[i].Mono = (MonoBehaviour) EditorGUILayout.ObjectField("New Option ",movementOptions[i].Mono, typeof(MonoBehaviour), true);
                }
                else
                {

                    bool accelerationSettingToggled = enableOptionRef.boolValue;
                    GUILayout.BeginHorizontal();
                    expandOptionRef.boolValue = SetupUtilities.DrawToggleHeaderFoldout(Styles.AccelerationSettings(monoRef.name), expandOptionRef.boolValue, ref accelerationSettingToggled, 0f);
                    if (GUILayout.Button("Remove"))
                    {
                        toRemove = i;
                    }
                    GUILayout.EndHorizontal();
                    ++EditorGUI.indentLevel;
                    if (expandOptionRef.boolValue)
                    {
                        EditorGUI.BeginDisabledGroup(!enableOptionRef.boolValue);
                        ShowAccelerationSettings((MonoBehaviour)monoRef.objectReferenceValue);
                        EditorGUI.EndDisabledGroup();
                    }
                    --EditorGUI.indentLevel;
                    EditorGUILayout.Space ();
                    enableOptionRef.boolValue = accelerationSettingToggled;
                    
                }
                
            }

            if (toRemove >= 0)
            {
                movementOptions.RemoveAt(toRemove);
            }
            if (!movementOptions.Any() || movementOptions.Last().Mono != null)
            {
                if (GUILayout.Button("Add Option"))
                {
                    movementOptions.Add(new InspectorOption());
                }
            }


            //so.ApplyModifiedProperties();
        }

        public void OnGUI()
        {
            testInt = EditorGUILayout.IntField("exp", testInt);
            testOpt.Mono = (MonoBehaviour)EditorGUILayout.ObjectField(testOpt.Mono, typeof(MonoBehaviour), true);
            
            if (GUILayout.Button("add magic"))
            {
                var newOpt = new InspectorOption();
                newOpt.Mono = (MonoBehaviour)EditorGUILayout.ObjectField(newOpt.Mono, typeof(MonoBehaviour), true);
            }
            //ScriptableObject target = this;
            //SerializedObject so = new SerializedObject(target);
            //SerializedProperty movementOptionsProperty = so.FindProperty("movementOptions");
            //MovementOptions[0] = (MonoBehaviour) EditorGUILayout.ObjectField("XR Origin",MovementOptions[0], typeof(MonoBehaviour), true);
            //MovementOptions[1] = (MonoBehaviour) EditorGUILayout.ObjectField("XR Origin",MovementOptions[1], typeof(MonoBehaviour), true);
            int toRemove = -1;
            for (int i = 0; i < movementOptions.Count; i++)
            {
                InspectorOption option = movementOptions[i];
                if (option.Mono == null)
                {
                    movementOptions[i].Mono = (MonoBehaviour) EditorGUILayout.ObjectField("New Option ",movementOptions[i].Mono, typeof(MonoBehaviour), true);
                }
                else
                {

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
                        ShowAccelerationSettings(option.Mono);
                        EditorGUI.EndDisabledGroup();
                    }
                    --EditorGUI.indentLevel;
                    EditorGUILayout.Space ();
                    option.enableOption = accelerationSettingToggled;
                    
                }
                
            }

            if (toRemove >= 0)
            {
                movementOptions.RemoveAt(toRemove);
            }
            if (!movementOptions.Any() || movementOptions.Last().Mono != null)
            {
                if (GUILayout.Button("Add Option"))
                {
                    movementOptions.Add(new InspectorOption());
                }
            }
            
        }

        private void ShowAccelerationSettings(MonoBehaviour option)
        {
            EditorGUILayout.LabelField("Level", "banana");
            //MovementOptions= EditorGUILayout.ObjectField("XR Origin",MovementOptions, typeof(MonoBehaviour), true);
            if (movementOptions != null)
            {
                Editor testEditor = Editor.CreateEditor(option);
                //testEditor.DrawDefaultInspector();
                testEditor.OnInspectorGUI();
            }
        }

        
    }
}