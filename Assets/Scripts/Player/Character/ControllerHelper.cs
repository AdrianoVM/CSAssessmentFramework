using System;
using System.Collections.Generic;
using System.Linq;
using Player.Movement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

namespace Player.Character
{
    public class ControllerHelper : MonoBehaviour
    {
        [Header("Actions with Button Values")]
        [SerializeField] private InputActionReference triggerValue;
        [SerializeField] private InputActionReference gripValue;
        [SerializeField] private InputActionReference primaryButton;
        [SerializeField] private InputActionReference secondaryButton;
        [SerializeField] private InputActionReference joystickValue;
        [Header("Actions Related to Buttons")]
        [SerializeField] private List<InputActionReference> triggerRelatedActions;
        [SerializeField] private List<InputActionReference> gripRelatedActions;
        [SerializeField] private List<InputActionReference> primaryBRelatedActions;
        [SerializeField] private List<InputActionReference> secondaryBRelatedActions;
        [SerializeField] private List<InputActionReference> joystickRelatedActions;

        // TODO: add a way to change materials of buttons to signify if they are enabled
        [SerializeField] private Material enabledMaterial;
        [SerializeField] private Material disabledMaterial;

        [Header("Button Renderers")]
        [SerializeField] private Renderer triggerRenderer;
        [SerializeField] private Renderer gripRenderer;
        [SerializeField] private Renderer primaryBRenderer;
        [SerializeField] private Renderer secondaryBRenderer;
        [SerializeField] private Renderer joystickRenderer;
        

        /// <summary>
        /// The animator component that contains the controller animation controller for animating buttons and triggers.
        /// </summary>
        private Animator _animator;

        private static readonly int JoyX = Animator.StringToHash("Joy X");
        private static readonly int JoyY = Animator.StringToHash("Joy Y");
        private static readonly int PrimaryB = Animator.StringToHash("Primary B");
        private static readonly int Property = Animator.StringToHash("Secondary B");
        private static readonly int Grip = Animator.StringToHash("Grip");
        private static readonly int Trigger = Animator.StringToHash("Trigger");
        

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            ExtendedControllerManager.ActionsModified += OnActionsModified;
        }

        private void OnDisable()
        {
            ExtendedControllerManager.ActionsModified -= OnActionsModified;
        }

        private void OnActionsModified(object sender, EventArgs e)
        {
            UpdateMaterial(triggerRenderer, triggerRelatedActions);
            UpdateMaterial(gripRenderer, gripRelatedActions);
            UpdateMaterial(primaryBRenderer, primaryBRelatedActions);
            UpdateMaterial(secondaryBRenderer, secondaryBRelatedActions);
            UpdateMaterial(joystickRenderer, joystickRelatedActions);
        }

        private void UpdateMaterial(Renderer buttonRenderer, List<InputActionReference> relatedActions)
        {
            if (buttonRenderer != null && enabledMaterial != null && disabledMaterial != null)
            {
                if (relatedActions.Any(r => r.action.enabled))
                {
                    if (buttonRenderer.material != enabledMaterial)
                    {
                        buttonRenderer.material = enabledMaterial;
                    }
                }
                else
                {
                    if (buttonRenderer.material != disabledMaterial)
                    {
                        buttonRenderer.material = disabledMaterial;
                    }
                }
            }
        }


        private void Update()
        {
            //could use a event system instead of checking every frame
            if (_animator != null)
            {
                if (joystickValue != null)
                {
                    _animator.SetFloat(JoyX, joystickValue.action.ReadValue<Vector2>().x);
                    _animator.SetFloat(JoyY, joystickValue.action.ReadValue<Vector2>().y);
                }

                if (triggerValue != null)
                {
                    _animator.SetFloat(Trigger, triggerValue.action.ReadValue<float>());
                }

                if (gripValue != null)
                {
                    _animator.SetFloat(Grip, gripValue.action.ReadValue<float>());
                }

                if (primaryButton != null)
                {
                    _animator.SetFloat(PrimaryB, primaryButton.action.IsPressed() ? 1 : 0);
                }

                if (secondaryButton != null)
                {
                    _animator.SetFloat(Property, secondaryButton.action.IsPressed() ? 1 : 0);
                }
            }
        }
        
    }
}
