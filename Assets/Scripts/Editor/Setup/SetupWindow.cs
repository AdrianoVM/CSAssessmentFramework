using System.Linq;
using Options;
using Options.Managers;
using ScriptableObjects;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        private SerializedObject[] _managerSOList;

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += StateChanged;
            EditorSceneManager.sceneOpened += SceneChanged;
            
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
            //FindManagers();
            
            // To refresh the UI
            Repaint();
        }

        /// <summary>
        /// Is Called by <see cref="EditorApplication.playModeStateChanged"/> event
        /// </summary>
        /// <param name="scene"> discarded</param>
        /// <param name="mode">discarded</param>
        private void SceneChanged(Scene scene, OpenSceneMode mode)
        {
            FindManagers();
        }

        /// <summary>
        /// Finds all the managers in the current scene.
        /// </summary>
        private void FindManagers()
        {
            _managers = FindObjectsOfType<Manager>();
            _managerEditors = _managers.Select(i => Editor.CreateEditor(i)).ToArray();
            _managerSOList = new SerializedObject[_managers.Length];
            for (var i = 0; i < _managers.Length; i++)
            {
                _managerSOList[i] = new SerializedObject(_managers[i]);
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

            if (_managers.Length < 1)
            {
                GUILayout.Label("No Managers in Scene", EditorStyles.largeLabel);
                return;
            }
            
            info = (GlobalInfo)EditorGUILayout.ObjectField("Global Info", info, typeof(GlobalInfo), false);

            foreach (SerializedObject serializedObject in _managerSOList)
            {
                serializedObject.Update();
            }

            if (FileEditorTools.FileButtons(info, _managerSOList, ref _showButtons))
            {
                foreach (SerializedObject serializedObject in _managerSOList)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            _selectedTab = GUILayout.Toolbar(_selectedTab, _managers.Select(i => i.managerName).ToArray(),GUILayout.MinHeight(30));
            EditorGUILayout.EndVertical();
            SetupUtilities.DrawSeparatorLine();
            if (_managerEditors.Length <= _selectedTab)
            {
                _selectedTab = 0;
                FindManagers();
            }
            else
            {
                _managerEditors[_selectedTab].OnInspectorGUI();
            }

        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= StateChanged;
            EditorSceneManager.sceneOpened -= SceneChanged;
        }
    }
}
