using System;
using Options.Movement;
using Setup;
using UnityEditor;
using UnityEngine;

namespace Options
{
    [CustomEditor(typeof(LocomotionHandler))]
    public class LocomotionHandlerEditor : CustomBaseEditor
    {
        private LocomotionHandler _handler;
        private SerializedProperty _leftHandLocomotionType;
        private SerializedProperty _rightHandLocomotionType;
        private SerializedProperty _leftHandTurnType;
        private SerializedProperty _rightHandTurnType;
        private SerializedProperty _dynamicMoveProvider;
        private SerializedProperty _teleportationProvider;
        private SerializedProperty _grabMoveProvider;
        private SerializedProperty _snapTurnProvider;
        private SerializedProperty _continuousTurnProvider;

        private SerializedProperty _leftHandControllerManager;
        private SerializedProperty _rightHandControllerManager;

        private void OnEnable()
        {
            //_handler = (LocomotionHandler)target;
            _leftHandLocomotionType = serializedObject.FindProperty("leftHandLocomotionType");
            _rightHandLocomotionType = serializedObject.FindProperty("rightHandLocomotionType");
            _leftHandTurnType = serializedObject.FindProperty("leftHandTurnType");
            _rightHandTurnType = serializedObject.FindProperty("rightHandTurnType");
            _dynamicMoveProvider = serializedObject.FindProperty("dynamicMoveProvider");
            _teleportationProvider = serializedObject.FindProperty("teleportationProvider");
            _grabMoveProvider = serializedObject.FindProperty("grabMoveProvider");
            _snapTurnProvider = serializedObject.FindProperty("snapTurnProvider");
            _continuousTurnProvider = serializedObject.FindProperty("continuousTurnProvider");
            _leftHandControllerManager = serializedObject.FindProperty("leftHandControllerManager");
            _rightHandControllerManager = serializedObject.FindProperty("rightHandControllerManager");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawNonEditableScriptReference<LocomotionHandler>();
            EditorGUILayout.PropertyField(_dynamicMoveProvider);
            EditorGUILayout.PropertyField(_teleportationProvider);
            EditorGUILayout.PropertyField(_grabMoveProvider);
            EditorGUILayout.PropertyField(_snapTurnProvider);
            EditorGUILayout.PropertyField(_continuousTurnProvider);
            EditorGUILayout.PropertyField(_leftHandControllerManager);
            EditorGUILayout.PropertyField(_rightHandControllerManager);
                
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("");
            GUILayout.Label("Left Hand");
            GUILayout.Label("Right Hand");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Locomotion Type");
            EditorGUILayout.PropertyField(_leftHandLocomotionType, EditorGUIUtility.TrTextContent(""));
            EditorGUILayout.PropertyField(_rightHandLocomotionType, EditorGUIUtility.TrTextContent(""));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Turn Type");
            EditorGUILayout.PropertyField(_leftHandTurnType, EditorGUIUtility.TrTextContent(""));
            EditorGUILayout.PropertyField(_rightHandTurnType, EditorGUIUtility.TrTextContent(""));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            SetupUtilities.DrawSeparatorLine(16);
            EditorGUILayout.Space();
            //Render the corresponding options
            var lLocoType = (LocomotionHandler.MovementType)Enum.GetValues(typeof(LocomotionHandler.MovementType)).GetValue(_leftHandLocomotionType.enumValueIndex);
            var rLocoType = (LocomotionHandler.MovementType)Enum.GetValues(typeof(LocomotionHandler.MovementType)).GetValue(_rightHandLocomotionType.enumValueIndex);
            var lTurnType = (LocomotionHandler.MovementType)Enum.GetValues(typeof(LocomotionHandler.MovementType)).GetValue(_leftHandTurnType.enumValueIndex);
            var rTurnType = (LocomotionHandler.MovementType)Enum.GetValues(typeof(LocomotionHandler.MovementType)).GetValue(_rightHandTurnType.enumValueIndex);
            
            //TODO: change it to show relevant options instead of rendering the editors
            if (_dynamicMoveProvider.objectReferenceValue != null)
            {
                if (lLocoType == LocomotionHandler.MovementType.Continuous || rLocoType == LocomotionHandler.MovementType.Continuous)
                {
                    EditorGUILayout.LabelField("Continuous Locomotion Options", EditorStyles.boldLabel);
                    //TODO: STOP CREATING EDITORS EVERY FRAME
                    //CreateEditor(_dynamicMoveProvider.objectReferenceValue).OnInspectorGUI();
                    EditorGUILayout.Space();
                    SetupUtilities.DrawSeparatorLine(16);
                    EditorGUILayout.Space();
                }
                
            }
            
            if (_teleportationProvider.objectReferenceValue != null)
            {
                if (lLocoType == LocomotionHandler.MovementType.Discrete || rLocoType == LocomotionHandler.MovementType.Discrete)
                {
                    EditorGUILayout.LabelField("Discrete Locomotion Options", EditorStyles.boldLabel);
                    //CreateEditor(_teleportationProvider.objectReferenceValue).OnInspectorGUI();
                    EditorGUILayout.Space();
                    SetupUtilities.DrawSeparatorLine(16);
                    EditorGUILayout.Space();
                }
            }
            
            if (_grabMoveProvider.objectReferenceValue != null)
            {
                if (false)
                {
                    EditorGUILayout.LabelField("Discrete Locomotion Options", EditorStyles.boldLabel);
                    CreateEditor(_teleportationProvider.objectReferenceValue).OnInspectorGUI();
                }
            }
            
            if (_snapTurnProvider.objectReferenceValue != null)
            {
                if (lTurnType == LocomotionHandler.MovementType.Discrete || rTurnType == LocomotionHandler.MovementType.Discrete)
                {
                    EditorGUILayout.LabelField("Discrete Turn Options", EditorStyles.boldLabel);
                    //CreateEditor(_snapTurnProvider.objectReferenceValue).OnInspectorGUI();
                    EditorGUILayout.Space();
                    SetupUtilities.DrawSeparatorLine(16);
                    EditorGUILayout.Space();
                }
            }
            
            if (_continuousTurnProvider.objectReferenceValue != null)
            {
                if (lTurnType == LocomotionHandler.MovementType.Continuous || rTurnType == LocomotionHandler.MovementType.Continuous)
                {
                    EditorGUILayout.LabelField("Continuous Turn Options", EditorStyles.boldLabel);
                    //CreateEditor(_snapTurnProvider.objectReferenceValue).OnInspectorGUI();
                    EditorGUILayout.Space();
                    SetupUtilities.DrawSeparatorLine(16);
                    EditorGUILayout.Space();
                }
            }
            
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}