using System;
using Options.Managers;
using Setup;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Options
{
    [CustomEditor(typeof(Manager), true)]
    public class ManagerEditor : Editor
    {
        
        private Vector2 _scrollPos;

        private static class Styles
        {
            public static GUIContent ToggleTitleStyle(string title)
            {
                return EditorGUIUtility.TrTextContent(title, "Toggle to enable or disable changing the settings.");
            } 
            
        }

        private SerializedProperty _options;
        private SerializedProperty _globalInfo;
        private SerializedProperty _managerName;
        private SerializedProperty _showFileButtons;

        private Editor[] _editors = new Editor[0];
        
        private void OnEnable()
        {
            _options = serializedObject.FindProperty("options");
            _showFileButtons = serializedObject.FindProperty("showFileButtons");
            // I don't know for sure which way is best, searching with string or with nameof.
            _globalInfo = serializedObject.FindProperty(nameof(Manager.globalInfo));
            _managerName = serializedObject.FindProperty(nameof(Manager.managerName));
            CreateEditors();
        }

        /// <summary>
        /// Creates the editors for each option in <see cref="_options"/>.
        /// </summary>
        private void CreateEditors()
        {
            foreach (Editor editor in _editors)
            {
                DestroyImmediate(editor);
            }
            _editors = new Editor[_options.arraySize];
            for (var i = 0; i < _options.arraySize; i++)
            {
                SerializedProperty monoP = _options.GetArrayElementAtIndex(i).FindPropertyRelative("monoBehaviour");
                if (monoP.objectReferenceValue != null)
                {
                    _editors[i] = CreateEditor(monoP.objectReferenceValue);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var myManager = (Manager) target;

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            EditorGUILayout.PropertyField(_globalInfo);
            EditorGUILayout.PropertyField(_managerName);

            var buttonsValue = _showFileButtons.boolValue;
            FileEditorTools.FileButtons(myManager.globalInfo, serializedObject, ref buttonsValue);
            _showFileButtons.boolValue = buttonsValue;
            SetupUtilities.DrawSeparatorLine();
            
            RenderInspectors();
            EditorGUILayout.EndScrollView();
            
            serializedObject.ApplyModifiedProperties();
            
            //apply all the setters
            foreach (InspectorOption option in myManager.options)
            {
                option.UpdateMonoInfo();
            }
        }

        /// <summary>
        /// Renders the inspectors of all the components in <see cref="_options"/>
        /// </summary>
        private void RenderInspectors()
        {
            //TODO: Add a way to add dismissible missing MonoBehaviour notification
            var toRemove = -1;
            var reCreate = false;
            for (var i = 0; i < _options.arraySize; i++)
            {
                SerializedProperty optionP = _options.GetArrayElementAtIndex(i);
                // if the optionP points to nothing, then it should be removed
                if (optionP.managedReferenceValue == null)
                {
                    toRemove = i;
                    break;
                }
                
                SerializedProperty expandOption = optionP.FindPropertyRelative("expandOption");
                SerializedProperty enableOption = optionP.FindPropertyRelative("enableOption");
                SerializedProperty mono = optionP.FindPropertyRelative("monoBehaviour");
                SerializedProperty monoName = optionP.FindPropertyRelative("monoName");
                SerializedProperty monoTypeName = optionP.FindPropertyRelative("monoTypeName");
                
                SetupUtilities.DrawSeparatorLine();
                
                if (mono.objectReferenceValue == null)
                {
                    GUILayout.Label(monoName.stringValue);
                    
                    // we use monoName in the preset system to check if mono has a null ref because the component was not found.
                    if (monoName.stringValue != new InspectorOption().monoName)
                    {
                        EditorGUILayout.HelpBox("Missing MonoBehaviour of type " + Type.GetType(monoTypeName.stringValue)?.Name.CamelCaseToSpaces(), MessageType.Warning);
                    }
                    
                    var addedMono = (MonoBehaviour)EditorGUILayout.ObjectField(Type.GetType(monoTypeName.stringValue)?.Name.CamelCaseToSpaces(), 
                        mono.objectReferenceValue, typeof(MonoBehaviour), true);
                     
                     //Some checks to not add weird interactions
                    if (addedMono != null)
                    {
                        var alreadyIn = false;
                        for (var j = 0; j < _options.arraySize; j++)
                        {
                            if (_options.GetArrayElementAtIndex(j).FindPropertyRelative("monoBehaviour")
                                    .objectReferenceValue == addedMono)
                            {
                                alreadyIn = true;
                                break;
                            }
                        }
                        if (addedMono is Manager)
                        {
                            Debug.LogWarning("Do not try Infinite Recursion");
                        }
                        else if (alreadyIn)
                        {
                            Debug.LogWarning("Inspector Is Already In the List");
                        }
                        else
                        {
                            mono.objectReferenceValue = addedMono;
                            monoName.stringValue = addedMono.name;
                            // to update the info of option, as the SerializedProperty only gets applied in the end
                            monoTypeName.stringValue = addedMono.GetType().AssemblyQualifiedName;
                            reCreate = true;
                        }
                    }
                    
                    if (GUILayout.Button("Remove"))
                    {
                        toRemove = i;
                    }
                }
                else
                {
                    // there is a reference, so there is something to render.
                    
                    // To select the corresponding gameObject in hierarchy.
                    var selectStyle = new GUIStyle();
                    selectStyle.fixedWidth = 20;
                    selectStyle.fixedHeight = 20;
                    GUILayout.BeginHorizontal();
                    var selected = GUILayout.Button(EditorGUIUtility.IconContent("GameObject Icon", "|Select target"),
                        selectStyle);
                    EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(),MouseCursor.Link);
                    GUILayout.Label(monoName.stringValue);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    if (selected)
                    {
                        Selection.SetActiveObjectWithContext(mono.objectReferenceValue, null);
                    }
                    
                    EditorGUILayout.Space();
                    
                    var optionEnabled = enableOption.boolValue;
                    var optionExpanded = expandOption.boolValue;
                    GUILayout.BeginHorizontal();
                    
                    SetupUtilities.DrawToggleHeaderFoldout(Styles.ToggleTitleStyle(Type.GetType(monoTypeName.stringValue)?.Name.CamelCaseToSpaces()),
                        ref optionExpanded, ref optionEnabled,0f);
                    
                    if (GUILayout.Button("Remove", GUILayout.MaxWidth(100)))
                    {
                        toRemove = i;
                    }

                    GUILayout.EndHorizontal();
                    ++EditorGUI.indentLevel;
                    if (optionExpanded)
                    {
                        EditorGUI.BeginDisabledGroup(!enableOption.boolValue);
                        if (mono.objectReferenceValue != null)
                        {
                            try
                            {
                                _editors[i].OnInspectorGUI();  //render inspector
                            }
                            catch (Exception)
                            {
                                // When entering play mode, while Setup window is closed,
                                // editors are destroyed and recreated, which sometimes causes issues
                                reCreate = true;
                            }
                        }
                        EditorGUI.EndDisabledGroup();
                    }

                    --EditorGUI.indentLevel;
                    EditorGUILayout.Space();
                    expandOption.boolValue = optionExpanded;
                    enableOption.boolValue = optionEnabled;
                }
            }

            if (toRemove >= 0)
            {
                // Disables the MonoBehaviour before removing it from the list
                var mono = _options.GetArrayElementAtIndex(toRemove).FindPropertyRelative("monoBehaviour")?.objectReferenceValue as MonoBehaviour;
                if (mono != null)
                {
                    mono.enabled = false;
                }
                // This function makes an element null if it exists instead of deleting.
                // apparently, but not how it works for me. May cause issues. 
                _options.DeleteArrayElementAtIndex(toRemove);
                reCreate = true;
            }

            if (_options.arraySize == 0 || _options.GetArrayElementAtIndex(_options.arraySize-1).FindPropertyRelative("monoBehaviour")?.objectReferenceValue != null)
            {
                if (GUILayout.Button("Add Option"))
                {
                    _options.arraySize++;
                    _options.GetArrayElementAtIndex(_options.arraySize - 1).managedReferenceValue =
                        new InspectorOption();
                    reCreate = true;
                }
            }

            if (reCreate)
            {
                CreateEditors();
            }
        }

        private void OnDisable()
        {
            foreach (Editor editor in _editors)
            {
                DestroyImmediate(editor);
            }
        }
    }
}