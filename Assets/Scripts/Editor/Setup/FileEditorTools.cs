using System;
using System.Collections.Generic;
using System.IO;
using Options;
using Options.Managers;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Setup
{
    public static class FileEditorTools
    {
        public static void FileButtons(GlobalInfo info, SerializedObject manager, ref bool show)
        {
            FileButtonsInternal(info, new[]{manager}, true, ref show);
        }

        public static void FileButtons(GlobalInfo info, SerializedObject[] managers, ref bool show)
        {
            FileButtonsInternal(info, managers, false, ref show);
        }
        
        private static void FileButtonsInternal(GlobalInfo info, SerializedObject[] managers, bool isUnique, ref bool showButtons)
        {
            showButtons = EditorGUILayout.Foldout(showButtons, "Save System", true);
            if (!showButtons)
            {
                return;
            }
            //var fileName = isUnique ? info.GetPathOfManager(managers[0].FindProperty("managerName").stringValue, true) : info.FilePath;
            var fileName = isUnique ? managers[0].FindProperty("managerPath").stringValue : info.FilePath;
            GUILayout.Label("Chosen File: "+Path.GetFileName(fileName), EditorStyles.largeLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load From", GUILayout.MaxWidth(100)))
            {
                var path = EditorUtility.OpenFilePanel("Where to Load From", "", "dat");
                if (path.Length != 0)
                {
                    if (info != null)
                    {
                        if (isUnique)
                        {
                            foreach (SerializedObject managerSO in managers)
                            {
                                var manager = (Manager)managerSO.targetObject;
                                managerSO.FindProperty("managerPath").stringValue = path;
                                managerSO.ApplyModifiedPropertiesWithoutUndo();
                                SerializedOptionsAction(managerSO, manager.LoadUnique);
                            }
                        }
                        else
                        {
                            info.FilePath = path;
                            foreach (SerializedObject manager in managers)
                            {
                                SerializedOptionsAction(manager, ((Manager)manager.targetObject).Load);
                            }
                            //managers.ForEach(m => m.Load());
                        }
                        
                    }
                    else
                    {
                        Debug.LogWarning("No Scriptable Object to save into");
                    }
                    
                }
            }

            if (GUILayout.Button("Save As", GUILayout.MaxWidth(100)))
            {
                var path = EditorUtility.SaveFilePanel("Where to Save", "", "OptionPreset", "dat");
                if (path.Length != 0)
                {
                    if (info != null)
                    {
                        if (isUnique)
                        {
                            foreach (SerializedObject managerSO in managers)
                            {
                                var manager = (Manager)managerSO.targetObject;
                                managerSO.FindProperty("managerPath").stringValue = path;
                                managerSO.ApplyModifiedPropertiesWithoutUndo();
                                SerializedOptionsAction(managerSO, manager.SaveUnique);
                            }
                        }
                        else
                        {
                            info.FilePath = path;
                            foreach (SerializedObject manager in managers)
                            {
                                SerializedOptionsAction(manager, ((Manager)manager.targetObject).Save);
                            }
                            //managers.ForEach(m => m.Save());
                        }
                        
                    }
                    else
                    {
                        Debug.LogWarning("No Scriptable Object to save into");
                    }
                    
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load",GUILayout.MaxWidth(100)))
            {
                if (isUnique)
                {
                    foreach (SerializedObject manager in managers)
                    {
                        SerializedOptionsAction(manager, ((Manager)manager.targetObject).LoadUnique);
                    }
                    
                    //managers.ForEach(m => m.LoadUnique());
                }
                else
                {
                    foreach (SerializedObject manager in managers)
                    {
                        SerializedOptionsAction(manager, ((Manager)manager.targetObject).Load);
                    }
                    //managers.ForEach(m => m.Load());
                }
                
            }
            if (GUILayout.Button("Save",GUILayout.MaxWidth(100)))
            {
                if (isUnique)
                {
                    foreach (SerializedObject manager in managers)
                    {
                        SerializedOptionsAction(manager, ((Manager)manager.targetObject).SaveUnique);
                    }
                    //managers.ForEach(m => m.SaveUnique());
                }
                else
                {
                    foreach (SerializedObject manager in managers)
                    {
                        SerializedOptionsAction(manager, ((Manager)manager.targetObject).Save);
                    }
                    //managers.ForEach(m => m.Save());
                }
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private static void SerializedOptionsAction(SerializedObject managerSO, Manager.ManagerDelegate managerDelegate)
        {
            SerializedProperty options = managerSO.FindProperty("options");
            var optionsList = new List<InspectorOption>();
            // Populate our temp list
            for (var i = 0; i < options.arraySize; i++)
            {
                optionsList.Add((InspectorOption) options.GetArrayElementAtIndex(i).managedReferenceValue);
            }
            managerDelegate(ref optionsList);
            options.ClearArray();
            options.arraySize = optionsList.Count;
            // repopulate our serialized list
            for (var i = 0; i < optionsList.Count; i++)
            {
                SerializedProperty optionSO = options.GetArrayElementAtIndex(i);
                optionSO.managedReferenceValue = optionsList[i];
                // This shouldn't need to exist, but the line above doesn't work outside of debug mode.
                optionSO.FindPropertyRelative("enableOption").boolValue = optionsList[i].EnableOption;
                optionSO.FindPropertyRelative("expandOption").boolValue = optionsList[i].expandOption;
                optionSO.FindPropertyRelative("monoBehaviour").objectReferenceValue = optionsList[i].Mono;
                optionSO.FindPropertyRelative("monoName").stringValue = optionsList[i].monoName;
                optionSO.FindPropertyRelative("MonoTypeName").stringValue = optionsList[i].MonoTypeName;

            }
        }

    }
}