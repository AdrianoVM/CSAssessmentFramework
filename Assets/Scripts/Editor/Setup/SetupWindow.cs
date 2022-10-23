
using System;
using System.Collections.Generic;
using System.Linq;
using Options;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Setup
{
    public class SetupWindow : EditorWindow
    {
        [MenuItem("CSFramework/SceneSetup")]
        public static void ShowWindow()
        {
            GetWindow<SetupWindow>("Scene Setup");
        }
        
        private int _selectedTab = 0;
        public Object testObject;
        // private MovementManager _movementManager;
        // private VisionManager _visionManager;
        // private EnvironmentManager _environmentManager;

        private Manager[] _managers;
        private Editor[] _managerEditors;
        private string[] _tabs =
        {
            "Movement",
            "Environment",
            "Vision"
        };
        [SerializeField]
        private MovementWindow movementMode;

        private void OnEnable()
        {
            _managers = FindObjectsOfType<Manager>();
            _managerEditors = _managers.Select(i => Editor.CreateEditor(i)).ToArray();
            if (movementMode == null)
            {
                movementMode = new MovementWindow();
            }
            Debug.Log("yes toi ");
        }

        private void OnGUI()
        {
            
            GUILayout.Label("Base Settings", EditorStyles.largeLabel);

            EditorGUILayout.BeginVertical();
            _selectedTab = GUILayout.Toolbar(_selectedTab, _managers.Select(i => i.ManagerName).ToArray(),GUILayout.MinHeight(30));
            EditorGUILayout.EndVertical();
            SetupUtilities.DrawSeparatorLine();
            _managerEditors[_selectedTab].OnInspectorGUI();

        }

        private void MovementTab()
        {
        
            EditorGUILayout.LabelField("Level", "banana");
        }
        private void EnvironmentTab()
        {
            EditorGUILayout.LabelField("Level", "banana2");
        }
        private void VisionTab()
        {
            EditorGUILayout.LabelField("Level", "banana");
        }
    }
}
