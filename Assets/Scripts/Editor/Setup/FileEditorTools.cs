using System.Collections.Generic;
using System.IO;
using System.Linq;
using Options;
using Options.Managers;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Setup
{
    public static class FileEditorTools
    {
        /// <summary>
        /// Renders buttons to save and load presets for managers
        /// </summary>
        /// <param name="info">ref to <see cref="GlobalInfo"/></param>
        /// <param name="manager">the Manager we want the buttons to apply to.</param>
        /// <param name="show">Whether the buttons are visible or not</param>
        /// <returns>Whether a button has been pressed.</returns>
        public static bool FileButtons(GlobalInfo info, SerializedObject manager, ref bool show)
        {
            return FileButtonsInternal(info, new[]{manager}, true, ref show);
        }

        /// <summary>
        /// Renders buttons to save and load presets for managers
        /// </summary>
        /// <param name="info">ref to <see cref="GlobalInfo"/></param>
        /// <param name="managers">the <see cref="Manager"/> list we want the buttons to apply to.</param>
        /// <param name="show">Whether the buttons are visible or not</param>
        /// <returns>Whether a button has been pressed.</returns>
        public static bool FileButtons(GlobalInfo info, SerializedObject[] managers, ref bool show)
        {
            return FileButtonsInternal(info, managers, false, ref show);
        }
        
        
        private static bool FileButtonsInternal(GlobalInfo info, SerializedObject[] managers, bool isUnique, ref bool showButtons)
        {
            showButtons = EditorGUILayout.Foldout(showButtons, "Save System", true);
            if (!showButtons)
            {
                return false;
            }

            var pressed = false;
            // if there is one manager, it has a parameter with info on path, otherwise GlobalInfo has it.
            var fileName = isUnique ? managers[0].FindProperty("managerPath").stringValue : info.FilePath;
            GUILayout.Label("Chosen File: "+Path.GetFileName(fileName), EditorStyles.largeLabel);
            
            // In each button, we call the corresponding method of the managers when the buttons are pressed.
            // pretty repetitive code
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load From", GUILayout.MaxWidth(100)))
            {
                pressed = true;
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
                pressed = true;
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
                pressed = true;
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
                pressed = true;
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
            return pressed;
        }

        /// <summary>
        /// Calls the given method in <paramref name="managerDelegate"/> and saves the returned values in <paramref name="managerSO"/>
        /// </summary>
        /// <param name="managerSO">the <see cref="SerializedObject"/> for the manager on which the delegate is called.</param>
        /// <param name="managerDelegate">The delegate of corresponding manager.</param>
        private static void SerializedOptionsAction(SerializedObject managerSO, Manager.ManagerDelegate managerDelegate)
        {
            // Dirtying does not work as expected with the Monobehaviours held by managers
            // as the modifications are not done on the SerializedObjects of these Monobehaviours.
            // Modifying this would be difficult but would be ideal.
            
            SerializedProperty options = managerSO.FindProperty("options");
            var optionsList = new List<InspectorOption>();
            // Populate our temp list
            for (var i = 0; i < options.arraySize; i++)
            {
                optionsList.Add((InspectorOption) options.GetArrayElementAtIndex(i).managedReferenceValue);
            }
            
            // Adding undo capability, which should be changed if SerializedObjects are used when modifying content of options.
            // This does not add the correct history to undo, but is enough to dirty the scene.
            // ctrl + Z still might not work
            Undo.RecordObjects(optionsList.Select(o => o.Mono as Object).ToArray(), "Preset Change");
            
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
                optionSO.FindPropertyRelative("monoTypeName").stringValue = optionsList[i].monoTypeName;
            }
        }
    }
}