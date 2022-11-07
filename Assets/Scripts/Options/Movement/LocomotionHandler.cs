using System;
using Player.Movement;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

namespace Options.Movement
{
    [ExecuteInEditMode]
    public class LocomotionHandler : MonoBehaviour
    {
        public enum MovementType
        {
            Continuous,
            Discrete,
            Disabled
        }
        
        // More complete version of ActionBasedContinuousMoveProvider
        [SerializeField] private DynamicMoveProvider dynamicMoveProvider;
        [SerializeField] private ActionBasedSnapTurnProvider snapTurnProvider;
        [SerializeField] private ActionBasedContinuousTurnProvider continuousTurnProvider;

        [SerializeField] private ExtendedControllerManager leftHandControllerManager;

        [SerializeField] private ExtendedControllerManager rightHandControllerManager;

        [SerializeField] private MovementType leftHandLocomotionType;

        public MovementType LeftHandLocomotionType
        {
            get => leftHandLocomotionType;
            set
            {
                UpdateLeftLocomotion(value, leftHandTurnType);
                leftHandLocomotionType = value;
            }
        }

        [SerializeField] private MovementType leftHandTurnType;

        public MovementType LeftHandTurnType
        {
            get => leftHandTurnType;
            set
            {
                UpdateLeftLocomotion(leftHandLocomotionType, value);
                leftHandTurnType = value;
            }
        }

        [SerializeField] private MovementType rightHandLocomotionType;
        public MovementType RightHandLocomotionType
        {
            get => rightHandLocomotionType;
            set
            {
                UpdateRightLocomotion(value, rightHandTurnType);
                rightHandLocomotionType = value;
            }
        }

        [SerializeField] private MovementType rightHandTurnType;

        public MovementType RightHandTurnType
        {
            get => rightHandTurnType;
            set
            {
                UpdateRightLocomotion(rightHandLocomotionType, value);
                rightHandTurnType = value;
            }
        }


        private void UpdateLeftLocomotion(MovementType motionType, MovementType turnType)
        {
            if (leftHandControllerManager == null)
            {
                return;
            }
            if (leftHandControllerManager.MotionType != motionType)
            {
                leftHandControllerManager.MotionType = motionType;
            }

            if (leftHandControllerManager.TurnType != turnType)
            {
                leftHandControllerManager.TurnType = turnType;
            }
            
        }
        
        private void UpdateRightLocomotion(MovementType motionType, MovementType turnType)
        {
            if (rightHandControllerManager == null)
            {
                return;
            }
            
            if (rightHandControllerManager.MotionType != motionType)
            {
                rightHandControllerManager.MotionType = motionType;
            }

            if (rightHandControllerManager.TurnType != turnType)
            {
                rightHandControllerManager.TurnType = turnType;
            }
        }

        private void OnValidate()
        {
            UpdateLeftLocomotion(leftHandLocomotionType, leftHandTurnType);
            UpdateRightLocomotion(rightHandLocomotionType, rightHandTurnType);
        }
    }
}