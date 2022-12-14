using System;
using System.Collections;
using Options.Gameplay;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Options.Vision
{
    [ExecuteInEditMode]
    public class DepthOfField : MonoBehaviour
    {
        [SerializeField] private Volume postProcessing;
        [Range(0.5f, 1.5f)] [SerializeField] private float blurIntensity = 1;
        [SerializeField] private bool constantBlur;
        [Range(0, 50)] [SerializeField] private float blurStartDistance = 10;
        [Range(0, 50)] [SerializeField] private float blurMaxDistance = 20;
        [SerializeField] private bool dynamicBlur;
        [Range(0, 50)] [SerializeField] private float dynamicBlurStartDistance = 0;
        [Range(0, 50)] [SerializeField] private float dynamicBlurMaxDistance = 0;

        private float _changedBlurStart;
        private float _changedBlurMax;
        private UnityEngine.Rendering.Universal.DepthOfField _depthOfField;
        private CharacterController _xrChara;

        private Vector3 _lastPos;
        private Quaternion _lastRot;
        private Vector3 _lastAngularVelocity;
        private int _changing;
        private Coroutine _changeBlurRoutine;
        
        //TODO: figure out simple way to get rid of non-dirtying
        private void OnEnable()
        {
            _changedBlurStart = constantBlur ? blurStartDistance : 500;
            _changedBlurMax = constantBlur ? blurMaxDistance : 500;
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
            if (constantBlur || dynamicBlur)
            {
                
                _depthOfField.active = true;
            }
            else
            {
                _depthOfField.active = false;
            }
            
            UpdateDof(_changedBlurStart, _changedBlurMax);
        }

        private void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            _xrChara = GameHandler.Instance.XROrigin.GetComponent<CharacterController>();

        }

        private void OnDisable()
        {
            if (postProcessing != null && _depthOfField != null)
            {
                //_depthOfField.intensity.value = _vignette.intensity.min;
                _depthOfField.active = false;
            }
        }


        private void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (dynamicBlur && GameHandler.State == GameHandler.StateType.Playing)
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
                if (_changing != (m || t ? 1 : -1))
                {
                    if(_changeBlurRoutine != null) StopCoroutine(_changeBlurRoutine);
                    _changeBlurRoutine = StartCoroutine(ChangeBlur(m || t, Time.time));
                }
                
            }
            
        }

        private IEnumerator ChangeBlur(bool moving, float time)
        {
            
            _changing = moving ? 1 : -1;
            var f = 0f;
            float startBlur;
            float maxBlur;
            while (f < 1)
            {
                f = Mathf.Clamp01(Time.time - time+0.2f);
                if (moving)
                {
                    startBlur = Mathf.Lerp(_depthOfField.gaussianStart.value, dynamicBlurStartDistance, f);
                    maxBlur = Mathf.Lerp(_depthOfField.gaussianEnd.value, dynamicBlurMaxDistance, f);
                }
                else
                {
                    startBlur = Mathf.Lerp(_depthOfField.gaussianStart.value, _changedBlurStart, f);
                    maxBlur = Mathf.Lerp(_depthOfField.gaussianEnd.value, _changedBlurMax, f);
                }
                UpdateDof(startBlur, maxBlur);
                yield return null;
            }
        }


        private void OnValidate()
        {
            if (postProcessing != null && _depthOfField != null)
            {
                _changedBlurStart = constantBlur ? blurStartDistance : 500;
                _changedBlurMax = constantBlur ? blurMaxDistance : 500;
                UpdateDof(_changedBlurStart, _changedBlurMax);
            }
        }
        

        private void UpdateDof(float startDist, float maxDist)
        {
            _depthOfField.gaussianStart.value = startDist;
            _depthOfField.gaussianEnd.value = maxDist;
            _depthOfField.gaussianMaxRadius.value = blurIntensity;
        }
        
        
    }
}