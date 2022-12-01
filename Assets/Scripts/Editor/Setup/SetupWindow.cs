using System.Linq;
using Options.Managers;
using ScriptableObjects;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        private bool _stateChangeHandled;

        public GlobalInfo info = null;
        private Manager[] _managers;
        private Editor[] _managerEditors = new Editor[0];
        private SerializedObject[] _managerSOList;

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += StateChanged;
            EditorSceneManager.sceneOpened += SceneChanged;
            _managers = new Manager[0];
            FindManagers();
        }

        private void OnHierarchyChange()
        {
            FindManagers();
            Repaint();
        }
        

        /// <summary>
        /// Updates <see cref="_currentState"/>.
        /// </summary>
        /// <param name="state">the (<see cref="PlayModeStateChange"/>) state of the editor.</param>
        private void StateChanged(PlayModeStateChange state)
        {
            _currentState = state;
            _stateChangeHandled = false;
            
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
            var managers = FindObjectsOfType<Manager>();
            //list comparison, is there a better way?
            var sameManagers = managers.Except(_managers).Count() < 1 && _managers.Except(managers).Count() < 1;
            if (!sameManagers || !_stateChangeHandled)
            {
                _managers = managers;
                foreach (Editor editor in _managerEditors)
                {
                    DestroyImmediate(editor);
                }
                _managerEditors = _managers.Select(i => Editor.CreateEditor(i)).ToArray();
                _managerSOList = new SerializedObject[_managers.Length];
                for (var i = 0; i < _managers.Length; i++)
                {
                    _managerSOList[i] = new SerializedObject(_managers[i]);
                }

                _stateChangeHandled = true;
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

            if (!_stateChangeHandled)
            {
                FindManagers();
                return;
            }

            if (_managers.Length < 1)
            {
                GUILayout.Label("No Managers in Scene", EditorStyles.largeLabel);
                return;
            }
            
            info = (GlobalInfo)EditorGUILayout.ObjectField("Global Info", info, typeof(GlobalInfo), false);

            if (info == null)
            {
                return;
            }
            
            foreach (SerializedObject serializedObject in _managerSOList)
            {
                if (serializedObject.targetObject == null)
                {
                    FindManagers();
                }
                else
                {
                    serializedObject.Update();
                }
                
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
            //SetupUtilities.DrawSeparatorLine();
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
            foreach (Editor editor in _managerEditors)
            {
                DestroyImmediate(editor);
            }
        }
    }
}
