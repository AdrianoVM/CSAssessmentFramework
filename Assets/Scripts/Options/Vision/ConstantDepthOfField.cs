using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Options.Vision
{
    [ExecuteInEditMode]
    public class ConstantDepthOfField : MonoBehaviour
    {
        [SerializeField] private Volume postProcessing;
        [Range(0, 50)] [SerializeField] private float blurStartDistance = 10;
        [Range(0, 50)] [SerializeField] private float blurMaxDistance = 20;
        private DepthOfField _depthOfField;
        
        private void OnEnable()
        {
            if (postProcessing == null)
            {
                Debug.LogWarning("No Post Processing set");
                return;
            }
            if (!postProcessing.profile.TryGet(out _depthOfField))
            {
                Debug.LogWarning("No Depth of Field found");
                return;
            }
            _depthOfField.active = true;
            UpdateDof();
        }

        private void OnDisable()
        {
            if (postProcessing != null && _depthOfField != null)
            {
                //_depthOfField.intensity.value = _vignette.intensity.min;
                _depthOfField.active = false;
            }
        }

        private void OnValidate()
        {
            if (postProcessing != null && _depthOfField != null)
            {
                UpdateDof();
            }
        }
        

        private void UpdateDof()
        {
            _depthOfField.gaussianStart.value = blurStartDistance;
            _depthOfField.gaussianEnd.value = blurMaxDistance;
        }
        
        
    }
}