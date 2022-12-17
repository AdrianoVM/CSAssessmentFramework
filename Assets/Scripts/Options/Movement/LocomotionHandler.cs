using JetBrains.Annotations;
using Options.Gameplay;
using Player.Movement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
// ReSharper disable InconsistentNaming

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
        
        // Providers are used in the custom editor
        
        // More complete version of ActionBasedContinuousMoveProvider
        [SerializeField] [UsedImplicitly] private ExtendedDynamicMoveProvider dynamicMoveProvider;
        [SerializeField] [UsedImplicitly] private TeleportationProvider teleportationProvider;
        [SerializeField] private TwoHandedGrabMoveProvider grabMoveProvider;
        [SerializeField] [UsedImplicitly] private ActionBasedSnapTurnProvider snapTurnProvider;
        [SerializeField] [UsedImplicitly] private ActionBasedContinuousTurnProvider continuousTurnProvider;

        [SerializeField] private ExtendedControllerManager leftHandControllerManager;

        [SerializeField] private ExtendedControllerManager rightHandControllerManager;

        // Left Hand
        [SerializeField] private MovementType leftHandLocomotionType;

        public MovementType LeftHandLocomotionType => leftHandLocomotionType;

        public void SetLeftHandLocomotionType(MovementType type)
        {
            UpdateLeftLocomotion(type, leftHandTurnType);
            leftHandLocomotionType = type;
        }
        
        public void SetLeftHandLocomotionType(int enumInt)
        {
            SetLeftHandLocomotionType((MovementType) enumInt);
        }
        

        [SerializeField] private MovementType leftHandTurnType;

        public MovementType LeftHandTurnType => leftHandTurnType;

        public void SetLeftHandTurnType(MovementType type)
        {
            UpdateLeftLocomotion(LeftHandLocomotionType, type);
            leftHandTurnType = type;
        }

        public void SetLeftHandTurnType(int enumInt)
        {
            SetLeftHandTurnType((MovementType) enumInt);
        }
        
        
        // Right Hand
        [SerializeField] private MovementType rightHandLocomotionType;
        public MovementType RightHandLocomotionType => rightHandLocomotionType;


        public void SetRightHandLocomotionType(MovementType type)
        {
            UpdateLeftLocomotion(type, rightHandTurnType);
            rightHandLocomotionType = type;
        }
        
        public void SetRightHandLocomotionType(int enumInt)
        {
            SetRightHandLocomotionType((MovementType) enumInt);
        }
        

        [SerializeField] private MovementType rightHandTurnType;

        public MovementType RightHandTurnType => rightHandTurnType;

        public void SetRightHandTurnType(MovementType type)
        {
            UpdateLeftLocomotion(rightHandLocomotionType, type);
            rightHandTurnType = type;
        }

        public void SetRightHandTurnType(int enumInt)
        {
            SetRightHandTurnType((MovementType) enumInt);
        }

        //Grab
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
        // They must exist here so that they are saved in JSON presets.
        // They must have the same name as the parameters they are controlling
        // I kind of hate this way of doing it. Please enhance if you can
        
        //ExtendedDynamicMoveProvider
        [SerializeField] [UsedImplicitly] private float m_MoveSpeed = 1f;
        [SerializeField] [UsedImplicitly] private bool m_EnableStrafe = true;
        [SerializeField] [UsedImplicitly] private ExtendedDynamicMoveProvider.MovementDirection m_LeftHandMovementDirection;
        [SerializeField] [UsedImplicitly] private ExtendedDynamicMoveProvider.MovementDirection m_RightHandMovementDirection;
        [SerializeField] [UsedImplicitly] private bool m_FixDownhill = true;
        
        //TeleportationProvider
        [SerializeField] [UsedImplicitly] private float m_DelayTime;
        
        //TwoHandedGrabMoveProvider
        [SerializeField] [UsedImplicitly] private float m_MoveFactor = 1;
        [SerializeField] [UsedImplicitly] private bool m_RequireTwoHandsForTranslation;
        [SerializeField] [UsedImplicitly] private bool m_EnableRotation;
        [SerializeField] [UsedImplicitly] private bool m_EnableScaling;
        
        //ActionBasedSnapTurnProvider
        [SerializeField] [UsedImplicitly] private float m_TurnAmount = 45f;
        [SerializeField] [UsedImplicitly] private float m_DebounceTime = 0.5f;
        [SerializeField] [UsedImplicitly] private bool m_EnableTurnAround = true;
        
        //ActionBasedContinuousTurnProvider
        [SerializeField] [UsedImplicitly] private float m_TurnSpeed = 60f;
        
        
        
        private void OnEnable()
        {
            GameHandler.GameStateChanged += GameStateChanged;
            GameStateChanged(GameHandler.State);
        }

        private void GameStateChanged(GameHandler.StateType state)
        {
            switch (state)
            {
                case GameHandler.StateType.Menu : case GameHandler.StateType.Pause:
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
                
                case GameHandler.StateType.Playing:
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


        /// <summary>
        /// Propagate updates to left hand <see cref="ExtendedControllerManager"/>
        /// </summary>
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
        
        /// <summary>
        /// Propagate updates to right hand <see cref="ExtendedControllerManager"/>
        /// </summary>
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
            GameHandler.GameStateChanged -= GameStateChanged;
        }
    }
}