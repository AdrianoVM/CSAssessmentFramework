using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Options.Vision
{
    [ExecuteInEditMode] // For showcase
    public class ConstantReducedFOV : MonoBehaviour
    {
        [SerializeField] private Volume postProcessing;
        [Range(0, 1)] [SerializeField] private float intensity;
        [Range(0.01f, 1)] [SerializeField] private float smoothness = 0.2f;
        private Vignette _vignette;

        private void OnEnable()
        {
            if (postProcessing == null)
            {
                Debug.LogWarning("No Post Processing set");
                return;
            }
            if (!postProcessing.profile.TryGet(out _vignette))
            {
                Debug.LogWarning("No vignette found");
                return;
            }

            UpdateFOV();
        }

        private void OnDisable()
        {
            if (postProcessing != null && _vignette != null)
            {
                _vignette.intensity.value = _vignette.intensity.min;
            }
        }

        private void OnValidate()
        {
            if (postProcessing != null && _vignette != null)
            {
                Debug.Log("fov val");
                UpdateFOV();
            }
        }

        private void UpdateFOV()
        {
            _vignette.intensity.value = intensity;
            _vignette.smoothness.value = smoothness;
        }
    }
}