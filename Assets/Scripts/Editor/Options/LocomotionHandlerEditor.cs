using System;
using Options.Movement;
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

        private void OnEnable()
        {
            _handler = (LocomotionHandler)target;
            _leftHandLocomotionType = serializedObject.FindProperty("leftHandLocomotionType");
            _rightHandLocomotionType = serializedObject.FindProperty("rightHandLocomotionType");
            _leftHandTurnType = serializedObject.FindProperty("leftHandTurnType");
            _rightHandTurnType = serializedObject.FindProperty("rightHandTurnType");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
            DrawNonEditableScriptReference<LocomotionHandler>();
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
            serializedObject.ApplyModifiedProperties();
        }
    }
}