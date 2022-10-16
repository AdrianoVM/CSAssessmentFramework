
using System;
using Unity.XR.CoreUtils;
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
            _selectedTab = GUILayout.Toolbar(_selectedTab, _tabs,GUILayout.MinHeight(30));
            EditorGUILayout.EndVertical();
            SetupUtilities.DrawSeparatorLine();
            switch (_selectedTab)
            {
                case 0:
                    movementMode.OnGUI();
                    
                    break;
                case 1:
                    EnvironmentTab();
                    break;
                case 2:
                    VisionTab();
                    break;
            }
        
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
