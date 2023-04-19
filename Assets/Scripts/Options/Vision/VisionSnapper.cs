using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Options.Gameplay;

namespace Options.Vision
{
    [ExecuteInEditMode]
    public class VisionSnapper : MonoBehaviour
    {
        [SerializeField] private Volume postProcessing;

        private UnityEngine.Rendering.Universal.ColorAdjustments _colorAdjustments;
        private ColorParameter colorFilter;

        private CharacterController _xrChara = null;
        private Quaternion _lastRot;
        private int _changing;
        private Coroutine _changeColourRoutine;


        private void OnEnable()
        {

            if (postProcessing == null)
            {
                Debug.LogWarning("No Post Processing set");
                return;
            }
            if (!postProcessing.profile.TryGet(out _colorAdjustments))
            {
                Debug.LogWarning("No Color Adjustments found");
                return;
            }

            _colorAdjustments.active = true;
            colorFilter = _colorAdjustments.colorFilter;
            colorFilter.overrideState = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            _xrChara = GameHandler.Instance.XROrigin.GetComponent<CharacterController>();
            _lastRot = _xrChara.transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {
            if (_xrChara != null && GameHandler.State == GameHandler.StateType.Playing)
            {
                var t = false;

                Quaternion rot = _xrChara.transform.rotation;
                Quaternion deltaRot = rot * Quaternion.Inverse(_lastRot);
                var eulerRot = deltaRot.eulerAngles;
                Vector3 angularVelocity = eulerRot / Time.fixedDeltaTime;

                // If snap turning
                t = angularVelocity.magnitude > 0;

                _lastRot = rot;
                if (_changing != (t ? 1 : -1))
                {
                    if (_changeColourRoutine != null) StopCoroutine(_changeColourRoutine);
                    _changeColourRoutine = StartCoroutine(ChangeColour(t));
                }

            }
        }

        private IEnumerator ChangeColour(bool turning)
        {
            _changing = turning ? 1 : -1;
            if (turning)
            {
                colorFilter.value = Color.black;
            }
            else
            {
                colorFilter.value = Color.white;
            }
            yield return null;
        }

        private void OnDisable()
        {
            if (postProcessing != null && _colorAdjustments != null)
            {
                _colorAdjustments.active = false;

            }
        }
    }
}

