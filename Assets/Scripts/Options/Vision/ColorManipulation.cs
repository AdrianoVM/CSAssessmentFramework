using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Options.Gameplay;

namespace Options.Vision
{ 
    public class ColorManipulation : MonoBehaviour
    {
        [SerializeField] private Volume postProcessing;
        [Range(0, 1)][SerializeField] private float redDegradation = 1f;
        [Range(0, 1)][SerializeField] private float greenDegradation = .1f;
        [Range(0, 1)][SerializeField] private float blueDegradation = 1f;
        [Range(0, 1)][SerializeField] private float saturationDegradation = 1f;

        private UnityEngine.Rendering.Universal.ColorCurves _colorCurves;
        private CharacterController _xrChara = null;
        private Quaternion _lastRot;
        private int _changing;
        private Coroutine _changeColourRoutine;

        private Keyframe[] keyFrames;
        private TextureCurveParameter redTex;
        private TextureCurveParameter greenTex;
        private TextureCurveParameter blueTex;
        private TextureCurveParameter masterTex;

        private void OnEnable()
        {
            
            if (postProcessing == null)
            {
                Debug.LogWarning("No Post Processing set");
                return;
            }
            if (!postProcessing.profile.TryGet(out _colorCurves))
            {
                Debug.LogWarning("No Color Curves found");
                return;
            }

            _colorCurves.active = true;
            masterTex = _colorCurves.master;
            masterTex.overrideState = true;
            redTex = _colorCurves.red;
            redTex.overrideState = true;
            greenTex = _colorCurves.green;
            greenTex.overrideState = true;
            blueTex = _colorCurves.blue;
            blueTex.overrideState = true;
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
            keyFrames = new Keyframe[8];
            keyFrames[0] = new Keyframe(1, redDegradation);
            keyFrames[1] = new Keyframe(1, greenDegradation);
            keyFrames[2] = new Keyframe(1, blueDegradation);
            keyFrames[3] = new Keyframe(1, saturationDegradation);
            // intial values
            keyFrames[4] = redTex.value[1];
            keyFrames[5] = greenTex.value[1];
            keyFrames[6] = blueTex.value[1];
            keyFrames[7] = masterTex.value[1];
        }

        // Update is called once per frame
        void Update()
        {
            if (GameHandler.State == GameHandler.StateType.Playing)
            {
                var t = false;

                Quaternion rot = _xrChara.transform.rotation;
                Quaternion deltaRot = rot * Quaternion.Inverse(_lastRot);
                var eulerRot = deltaRot.eulerAngles;
                Vector3 angularVelocity = eulerRot / Time.fixedDeltaTime;

                // If snap turning
                t = angularVelocity.magnitude > 0;

                _lastRot = rot;
                if (_changing != ( t ? 1 : -1))
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
                redTex.value.MoveKey(1, keyFrames[0]);
                greenTex.value.MoveKey(1, keyFrames[1]);
                blueTex.value.MoveKey(1, keyFrames[2]);
                masterTex.value.MoveKey(1, keyFrames[3]);
            }
            else
            {
                redTex.value.MoveKey(1,  keyFrames[4]);
                greenTex.value.MoveKey(1,  keyFrames[5]);
                blueTex.value.MoveKey(1,  keyFrames[6]);
                masterTex.value.MoveKey(1,  keyFrames[7]);
            }
            yield return null;
        }

        private void OnDisable()
        {
            if (postProcessing != null && _colorCurves != null)
            {
                _colorCurves.active = false;

            }
        }
    }
}

