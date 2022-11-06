using System;
using Options.Movement;
using UnityEditor;

namespace Options
{
    [CustomEditor(typeof(LocomotionHandler))]
    public class LocomotionHandlerEditor : CustomBaseEditor
    {
        private LocomotionHandler _handler;
        private SerializedProperty _leftHandTurnType;

        private void OnEnable()
        {
            _handler = (LocomotionHandler)target;
            _leftHandTurnType = serializedObject.FindProperty("leftHandTurnType");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
            DrawNonEditableScriptReference<LocomotionHandler>();
            EditorGUILayout.PropertyField(_leftHandTurnType, EditorGUIUtility.TrTextContent("t"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}