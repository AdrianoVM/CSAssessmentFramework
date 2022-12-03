using System;
using Options.Gameplay;
using Player.Movement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

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
        [SerializeField] private ExtendedDynamicMoveProvider dynamicMoveProvider;
        [SerializeField] private TeleportationProvider teleportationProvider;
        [SerializeField] private TwoHandedGrabMoveProvider grabMoveProvider;
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


        [SerializeField] private bool leftHandGrabMove;
        
        public bool LeftHandGrabMove
        {
            get => leftHandGrabMove;
            set => leftHandGrabMove = ChangeGrabMove(false, value);
        }

        [SerializeField] private bool rightHandGrabMove;


        public bool RightHandGrabMove
        {
            get => rightHandGrabMove;
            set => rightHandGrabMove = ChangeGrabMove(true, value);
        }

        private bool ChangeGrabMove(bool rightHand, bool value)
        {
            InputAction action;
            if (rightHand)
            {
                action = grabMoveProvider.rightGrabMoveProvider.grabMoveAction.reference.action;
            }
            else
            {
                action = grabMoveProvider.leftGrabMoveProvider.grabMoveAction.reference.action;
            }
            
            if (value)
            {
                action.Enable();
            }
            else
            {
                action.Disable();
            }

            ExtendedControllerManager.OnActionsModified();
            return value;
        }
        

        //list of parameters of various movement controllers, used by Editor
        // They must have the same name as the parameters they are controlling
        // I kind of hate this way of doing it. Please enhance if you can
        //ExtendedDynamicMoveProvider
        [SerializeField] private float m_MoveSpeed = 1f;
        [SerializeField] private bool m_EnableStrafe = true;
        [SerializeField] private ExtendedDynamicMoveProvider.MovementDirection m_LeftHandMovementDirection;
        [SerializeField] private ExtendedDynamicMoveProvider.MovementDirection m_RightHandMovementDirection;
        [SerializeField] private bool m_FixDownhill = true;
        
        //TeleportationProvider
        [SerializeField] private float m_DelayTime;
        
        //TwoHandedGrabMoveProvider
        [SerializeField] private float m_MoveFactor = 1;
        [SerializeField] private bool m_RequireTwoHandsForTranslation;
        [SerializeField] private bool m_EnableRotation;
        [SerializeField] private bool m_EnableScaling;
        
        //ActionBasedSnapTurnProvider
        [SerializeField] private float m_TurnAmount = 45f;
        [SerializeField] private float m_DebounceTime = 0.5f;
        [SerializeField] private bool m_EnableTurnAround = true;
        
        //ActionBasedContinuousTurnProvider
        [SerializeField] private float m_TurnSpeed = 60f;
        
        
        
        private void OnEnable()
        {
            GameManager.GameStateChanged += GameStateChanged;
            GameStateChanged(GameManager.State);
        }

        private void GameStateChanged(GameManager.StateType state)
        {
            Debug.Log(state);
            switch (state)
            {
                case GameManager.StateType.Menu : case GameManager.StateType.Pause:
                    if (grabMoveProvider != null)
                    {
                        ChangeGrabMove(false, false);
                        ChangeGrabMove(true, false);
                    }
                    
                    leftHandControllerManager.MotionType = MovementType.Disabled;
                    leftHandControllerManager.TurnType = MovementType.Disabled;

                    rightHandControllerManager.MotionType = MovementType.Disabled;
                    rightHandControllerManager.TurnType = MovementType.Disabled;
                    break;
                
                case GameManager.StateType.Playing:
                    if (grabMoveProvider != null)
                    {
                        ChangeGrabMove(false, LeftHandGrabMove);
                        ChangeGrabMove(true, RightHandGrabMove);
                    }
                    leftHandControllerManager.MotionType = LeftHandLocomotionType;
                    leftHandControllerManager.TurnType = LeftHandTurnType;

                    rightHandControllerManager.MotionType = RightHandLocomotionType;
                    rightHandControllerManager.TurnType = RightHandTurnType;
                    break;
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
            LeftHandGrabMove = leftHandGrabMove;
            RightHandGrabMove = rightHandGrabMove;
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameStateChanged;
        }
    }
}