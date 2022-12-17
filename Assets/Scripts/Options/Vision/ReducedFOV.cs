using System;
using System.Collections;
using Options.Gameplay;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Options.Vision
{
    [ExecuteInEditMode] // For showcase
    public class ReducedFOV : MonoBehaviour
    {
        // A lot of code is similar to DepthOfField, could add base class to clean up.
        // Both were done quickly so are not necessarily well implemented.
        [SerializeField] private Volume postProcessing;
        [Header("Constant FOV")]
        [SerializeField] private bool constantFOV;
        [Range(0, 1)] [SerializeField] private float intensity;
        [Range(0.01f, 1)] [SerializeField] private float smoothness = 0.2f;
        [Header("Dynamic FOV")]
        [SerializeField] private bool dynamicFOV;
        [SerializeField] private float transitionSpeed = 2f;
        [Range(0, 1)] [SerializeField] private float dynamicIntensity;
        [Range(0.01f, 1)] [SerializeField] private float dynamicSmoothness = 0.2f;
        
        
        private Vignette _vignette;
        private float _changedFOVIntensity;
        private float _changedFOVSmoothness;
        private CharacterController _xrChara;

        private Vector3 _lastPos;
        private Quaternion _lastRot;
        private Vector3 _lastAngularVelocity;
        private int _changing;
        private Coroutine _changeVignetteRoutine;

        private void OnEnable()
        {
            _changedFOVIntensity = constantFOV ? intensity : 0;
            _changedFOVSmoothness = constantFOV ? smoothness : 0;
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

            _vignette.active = constantFOV || dynamicFOV;

            UpdateFOV(_changedFOVIntensity, _changedFOVSmoothness);
        }

        private void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            _xrChara = GameHandler.Instance.XROrigin.GetComponent<CharacterController>();
        }
        
        private void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (dynamicFOV && GameHandler.State == GameHandler.StateType.Playing)
            {
                var m = false;
                var t = false;
                var v = _xrChara.velocity.magnitude;
                Vector3 pos = _xrChara.transform.position;
                //velocity sometimes stick to a value even when stopped
                if (v > 0.3 && pos != _lastPos)
                {
                    m = true;
                }

                _lastPos = pos;

                Quaternion rot = GameHandler.Instance.XROrigin.transform.rotation;
                Quaternion deltaRot = rot * Quaternion.Inverse(_lastRot);
                var eulerRot =  new Vector3( Mathf.DeltaAngle( 0, deltaRot.eulerAngles.x ), Mathf.DeltaAngle( 0, deltaRot.eulerAngles.y ),Mathf.DeltaAngle( 0, deltaRot.eulerAngles.z ) );
                Vector3 angularVelocity = eulerRot / Time.fixedDeltaTime;
                
                if (_lastAngularVelocity.magnitude > 10 && angularVelocity.magnitude > 10)
                {
                    t = true;
                }

                _lastAngularVelocity = angularVelocity;
                _lastRot = rot;
                // if we are not already changing the vignette to the same destination values.
                if (_changing != (m || t ? 1 : -1))
                {
                    if(_changeVignetteRoutine != null) StopCoroutine(_changeVignetteRoutine);
                    _changeVignetteRoutine = StartCoroutine(ChangeVignette(m || t));
                }
            }
        }
        
        /// <summary>
        /// Changes the vignette values to either the dynamic or constant values.
        /// </summary>
        /// <param name="moving">If true, changes to dynamic values of vignette.
        /// If false, changes to constant values of vignette. </param>
        private IEnumerator ChangeVignette(bool moving)
        {
            var time = Time.time;
            _changing = moving ? 1 : -1;
            var f = 0f;
            float fovIntensity;
            float fovSmooth;
            // we use intensity to measure progress.
            var currentI= (_vignette.intensity.value - _changedFOVIntensity)/(dynamicIntensity - _changedFOVIntensity);
            currentI = moving ? currentI : 1 - currentI;
            while (f < 1)
            {
                f = Mathf.Clamp01((Time.time - time)/transitionSpeed + currentI);
                if (moving)
                {
                    fovIntensity = Mathf.Lerp(_changedFOVIntensity, dynamicIntensity, f);
                    fovSmooth = Mathf.Lerp(_changedFOVSmoothness, dynamicSmoothness, f);
                }
                else
                {
                    fovIntensity = Mathf.Lerp(dynamicIntensity, _changedFOVIntensity, f);
                    fovSmooth = Mathf.Lerp(dynamicSmoothness, _changedFOVSmoothness, f);
                }
                UpdateFOV(fovIntensity, fovSmooth);
                yield return null;
            }
        }

        private void OnValidate()
        {
            if (postProcessing != null && _vignette != null)
            {
                _changedFOVIntensity = constantFOV ? intensity : 0;
                _changedFOVSmoothness = constantFOV ? smoothness : 0;
                _vignette.active = constantFOV || dynamicFOV;
                UpdateFOV(_changedFOVIntensity, _changedFOVSmoothness);
            }
        }

        private void UpdateFOV(float fovIntensity, float fovSmoothness)
        {
            _vignette.intensity.value = fovIntensity;
            _vignette.smoothness.value = fovSmoothness;
        }

        private void OnDisable()
        {
            if (postProcessing != null && _vignette != null)
            {
                _vignette.intensity.value = _vignette.intensity.min;
            }
        }
    }
}