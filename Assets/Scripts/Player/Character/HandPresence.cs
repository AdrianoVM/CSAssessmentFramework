using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Character
{
    /// <summary>
    /// Responsible for handling hand instantiation and animation.
    /// </summary>
    public class HandPresence : MonoBehaviour
    {
        [SerializeField] public GameObject handModelPrefab;
        [SerializeField] private InputActionReference triggerValue;
        [SerializeField] private InputActionReference gripValue;
    
        private GameObject _spawnedHandModel;
        private Animator _handAnimator;
        private static readonly int Trigger = Animator.StringToHash("Trigger");
        private static readonly int Grip = Animator.StringToHash("Grip");


        private void Start()
        {
            _spawnedHandModel = Instantiate(handModelPrefab, transform);
            _handAnimator = _spawnedHandModel.GetComponent<Animator>();
        }

        
        private void Update()
        {
            UpdateHandAnimation();
        }
    
        private void UpdateHandAnimation()
        {
            if (gripValue && triggerValue)
            {
                _handAnimator.SetFloat(Trigger, triggerValue.action.ReadValue<float>());
                _handAnimator.SetFloat(Grip, gripValue.action.ReadValue<float>());
                _handAnimator.SetLayerWeight(_handAnimator.GetLayerIndex("Point Layer"), Mathf.Max(gripValue.action.ReadValue<float>() - triggerValue.action.ReadValue<float>(), 0));
            }
            else
            {
                _handAnimator.SetFloat(Grip, 0);
                _handAnimator.SetFloat(Trigger, 0);
            }
        }

    }
}
