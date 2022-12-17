using System;
using Options.Movement;
using Setup;
using UnityEditor;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace Options
{
    /// <summary>
    /// Editor for LocomotionHandler,
    /// responsible for propagating the changes to corresponding serializedObject of locomotionProviders. <br/>
    /// This code is quite disturbing, please change if a better way is found.
    /// </summary>
    [CustomEditor(typeof(LocomotionHandler))]
    public class LocomotionHandlerEditor : CustomBaseEditor
    {
        private SerializedProperty _leftHandLocomotionType;
        private SerializedProperty _rightHandLocomotionType;
        private SerializedProperty _leftHandTurnType;
        private SerializedProperty _rightHandTurnType;
        private SerializedProperty _leftHandGrabMove;
        private SerializedProperty _rightHandGrabMove;
        private SerializedProperty _dynamicMoveProvider;
        private SerializedProperty _teleportationProvider;
        private SerializedProperty _grabMoveProvider;
        private SerializedProperty _snapTurnProvider;
        private SerializedProperty _continuousTurnProvider;

        private SerializedProperty _leftHandControllerManager;
        private SerializedProperty _rightHandControllerManager;
        
        //External Parameters
        //ExtendedDynamicMoveProvider
        private SerializedProperty _m_MoveSpeed;
        private SerializedProperty _m_EnableStrafe;
        private SerializedProperty _m_LeftHandMovementDirection;
        private SerializedProperty _m_RightHandMovementDirection;
        private SerializedProperty _m_FixDownhill;
        //TeleportationProvider
        private SerializedProperty _m_DelayTime;
        //TwoHandedGrabMoveProvider
        private SerializedProperty _m_MoveFactor;
        private SerializedProperty _m_RequireTwoHandsForTranslation;
        private SerializedProperty _m_EnableRotation;
        private SerializedProperty _m_EnableScaling;
        //ActionBasedSnapTurnProvider
        private SerializedProperty _m_TurnAmount;
        private SerializedProperty _m_DebounceTime;
        private SerializedProperty _m_EnableTurnAround;
        //ActionBasedContinuousTurnProvider
        private SerializedProperty _m_TurnSpeed;

        private void OnEnable()
        {
            _leftHandLocomotionType = serializedObject.FindProperty("leftHandLocomotionType");
            _rightHandLocomotionType = serializedObject.FindProperty("rightHandLocomotionType");
            _leftHandTurnType = serializedObject.FindProperty("leftHandTurnType");
            _rightHandTurnType = serializedObject.FindProperty("rightHandTurnType");
            _leftHandGrabMove = serializedObject.FindProperty("leftHandGrabMove");
            _rightHandGrabMove = serializedObject.FindProperty("rightHandGrabMove");
            _dynamicMoveProvider = serializedObject.FindProperty("dynamicMoveProvider");
            _teleportationProvider = serializedObject.FindProperty("teleportationProvider");
            _grabMoveProvider = serializedObject.FindProperty("grabMoveProvider");
            _snapTurnProvider = serializedObject.FindProperty("snapTurnProvider");
            _continuousTurnProvider = serializedObject.FindProperty("continuousTurnProvider");
            _leftHandControllerManager = serializedObject.FindProperty("leftHandControllerManager");
            _rightHandControllerManager = serializedObject.FindProperty("rightHandControllerManager");
            
            //External Parameters
            
            _m_MoveSpeed = serializedObject.FindProperty("m_MoveSpeed");
            _m_EnableStrafe = serializedObject.FindProperty("m_EnableStrafe");
            _m_LeftHandMovementDirection = serializedObject.FindProperty("m_LeftHandMovementDirection");
            _m_RightHandMovementDirection = serializedObject.FindProperty("m_RightHandMovementDirection");
            _m_FixDownhill = serializedObject.FindProperty("m_FixDownhill");
            _m_DelayTime = serializedObject.FindProperty("m_DelayTime");
            _m_MoveFactor = serializedObject.FindProperty("m_MoveFactor");
            _m_RequireTwoHandsForTranslation = serializedObject.FindProperty("m_RequireTwoHandsForTranslation");
            _m_EnableRotation = serializedObject.FindProperty("m_EnableRotation");
            _m_EnableScaling = serializedObject.FindProperty("m_EnableScaling");
            _m_TurnAmount = serializedObject.FindProperty("m_TurnAmount");
            _m_DebounceTime = serializedObject.FindProperty("m_DebounceTime");
            _m_EnableTurnAround = serializedObject.FindProperty("m_EnableTurnAround");
            _m_TurnSpeed = serializedObject.FindProperty("m_TurnSpeed");
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
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("GrabMove");
            EditorGUILayout.PropertyField(_leftHandGrabMove, EditorGUIUtility.TrTextContent(""));
            EditorGUILayout.PropertyField(_rightHandGrabMove, EditorGUIUtility.TrTextContent(""));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            SetupUtilities.DrawSeparatorLine(16);
            EditorGUILayout.Space();
            //Render the corresponding options
            var lLocoType = (LocomotionHandler.MovementType)Enum.GetValues(typeof(LocomotionHandler.MovementType)).GetValue(_leftHandLocomotionType.enumValueIndex);
            var rLocoType = (LocomotionHandler.MovementType)Enum.GetValues(typeof(LocomotionHandler.MovementType)).GetValue(_rightHandLocomotionType.enumValueIndex);
            var lTurnType = (LocomotionHandler.MovementType)Enum.GetValues(typeof(LocomotionHandler.MovementType)).GetValue(_leftHandTurnType.enumValueIndex);
            var rTurnType = (LocomotionHandler.MovementType)Enum.GetValues(typeof(LocomotionHandler.MovementType)).GetValue(_rightHandTurnType.enumValueIndex);
            
            if (_dynamicMoveProvider.objectReferenceValue != null)
            {
                if (lLocoType == LocomotionHandler.MovementType.Continuous || rLocoType == LocomotionHandler.MovementType.Continuous)
                {
                    EditorGUILayout.LabelField("Continuous Locomotion Options", EditorStyles.boldLabel);

                    var properties = new[] { _m_MoveSpeed, _m_EnableStrafe, _m_LeftHandMovementDirection, _m_RightHandMovementDirection, _m_FixDownhill };
                    var names = new[] { "m_MoveSpeed","m_EnableStrafe", "m_LeftHandMovementDirection", "m_RightHandMovementDirection", "m_FixDownhill"};
                    ShowOptions(_dynamicMoveProvider.objectReferenceValue,properties, names);
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
                    
                    var properties = new[] { _m_DelayTime };
                    var names = new[] { "m_DelayTime"};
                    ShowOptions(_teleportationProvider.objectReferenceValue,properties, names);
                    EditorGUILayout.Space();
                    SetupUtilities.DrawSeparatorLine(16);
                    EditorGUILayout.Space();
                }
            }

            if (_snapTurnProvider.objectReferenceValue != null)
            {
                if (lTurnType == LocomotionHandler.MovementType.Discrete || rTurnType == LocomotionHandler.MovementType.Discrete)
                {
                    EditorGUILayout.LabelField("Discrete Turn Options", EditorStyles.boldLabel);
                    var properties = new[] { _m_TurnAmount, _m_DebounceTime, _m_EnableTurnAround};
                    var names = new[] { "m_TurnAmount","m_DebounceTime", "m_EnableTurnAround"};
                    ShowOptions(_snapTurnProvider.objectReferenceValue,properties, names);
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
                    var properties = new[] { _m_TurnSpeed };
                    var names = new[] { "m_TurnSpeed"};
                    ShowOptions(_continuousTurnProvider.objectReferenceValue,properties, names);
                    EditorGUILayout.Space();
                    SetupUtilities.DrawSeparatorLine(16);
                    EditorGUILayout.Space();
                }
            }

            if (_grabMoveProvider.objectReferenceValue != null)
            {
                if (_leftHandGrabMove.boolValue || _rightHandGrabMove.boolValue)
                {
                    EditorGUILayout.LabelField("Grab Move Options", EditorStyles.boldLabel);
                    var properties = new[] { _m_MoveFactor, _m_RequireTwoHandsForTranslation, _m_EnableRotation, _m_EnableScaling };
                    var names = new[] { "m_MoveFactor","m_RequireTwoHandsForTranslation", "m_EnableRotation", "m_EnableScaling"};
                    ShowOptions(_grabMoveProvider.objectReferenceValue,properties, names);
                }
            }


            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Renders the propertyFields for the given <see cref="SerializedProperty"/>,
        /// then copy the content of these properties to the given <see cref="UnityEngine.Object"/>'s <see cref="SerializedObject"/>
        /// </summary>
        /// <param name="obj"> The object that contains the <see cref="SerializedProperty"/> in parameter <paramref name="properties"/>.</param>
        /// <param name="properties">The <see cref="SerializedProperty"/> for which the method renders a field
        /// and copies to the <see cref="SerializedObject"/> of parameter <paramref name="obj"/></param>
        /// <param name="propertyNames">The names corresponding to the <see cref="SerializedProperty"/> in parameter <paramref name="properties"/>.</param>
        private void ShowOptions(UnityEngine.Object obj, SerializedProperty[] properties,string[] propertyNames)
        {
            var so = new SerializedObject(obj);
            so.Update();
            for (int i = 0; i < propertyNames.Length; i++)
            {
                SerializedProperty sp = so.FindProperty(propertyNames[i]);
                
                if (sp != null)
                {
                    EditorGUILayout.PropertyField(properties[i]);
                    so.CopyFromSerializedPropertyIfDifferent(properties[i]);
                }
                
            }
            so.ApplyModifiedProperties();
        }
    }
}