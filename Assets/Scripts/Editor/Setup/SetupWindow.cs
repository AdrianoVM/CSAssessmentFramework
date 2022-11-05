
using System;
using System.IO;
using System.Linq;
using Options;
using UnityEditor;
using UnityEngine;
using Utilities;
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
        private bool _showButtons;

        private PlayModeStateChange _currentState;
        // private MovementManager _movementManager;
        // private VisionManager _visionManager;
        // private EnvironmentManager _environmentManager;

        public GlobalInfo info = null;
        private Manager[] _managers;
        private Editor[] _managerEditors;
        
        [SerializeField]
        private MovementWindow movementMode;

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += StateChanged;
            
            FindManagers();
        }

        private void OnHierarchyChange()
        {
            FindManagers();
            Repaint();
        }

        /// <summary>
        /// Updates <see cref="_currentState"/> and calls <see cref="FindManagers"/>.
        /// </summary>
        /// <param name="state">the (<see cref="PlayModeStateChange"/>) state of the editor.</param>
        private void StateChanged(PlayModeStateChange state)
        {
            _currentState = state;
            FindManagers();
            
            // To refresh the UI
            Repaint();
        }

        /// <summary>
        /// Finds all the managers in the current scene.
        /// </summary>
        private void FindManagers()
        {
            _managers = FindObjectsOfType<Manager>();
            _managerEditors = _managers.Select(i => Editor.CreateEditor(i)).ToArray();
            if (movementMode == null)
            {
                movementMode = new MovementWindow();
            }
            
        }
        

        private void OnGUI()
        {
            var headStyle = new GUIStyle();
            headStyle.fontSize = 20;
            headStyle.fontStyle = FontStyle.Bold;
            headStyle.normal.textColor = Color.white;
            
            GUILayout.Label("Setup", headStyle);
            //Avoid rendering when Managers are changing state.
            if (_currentState == PlayModeStateChange.ExitingEditMode || _currentState == PlayModeStateChange.ExitingPlayMode)
            {
                GUILayout.Label("Loading", EditorStyles.largeLabel);
                return;
            }
            

            
            info = (GlobalInfo)EditorGUILayout.ObjectField("Global Info", info, typeof(GlobalInfo), false);
            FileManager.FileButtons(info, _managers, ref _showButtons);

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            _selectedTab = GUILayout.Toolbar(_selectedTab, _managers.Select(i => i.managerName).ToArray(),GUILayout.MinHeight(30));
            EditorGUILayout.EndVertical();
            SetupUtilities.DrawSeparatorLine();
            if (_managerEditors.Length < _selectedTab)
            {
                Debug.LogError("Selected Manager Not found, Try refreshing scene");
            }
            else
            {
                _managerEditors[_selectedTab].OnInspectorGUI();
            }
            

        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= StateChanged;
        }
    }
}
