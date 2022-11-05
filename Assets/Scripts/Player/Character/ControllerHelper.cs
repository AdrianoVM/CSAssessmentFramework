using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Character
{
    public class ControllerHelper : MonoBehaviour
    {
        
        [SerializeField] private InputActionReference triggerValue;
        [SerializeField] private InputActionReference gripValue;
        [SerializeField] private InputActionReference primaryButton;
        [SerializeField] private InputActionReference secondaryButton;
        [SerializeField] private InputActionReference joystickValue;
        // TODO: add a way to change materials of buttons to signify if they are enabled

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
