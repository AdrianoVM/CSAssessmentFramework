using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Options.Gameplay;

namespace Options.Vision
{ 
    public class ColorBlur : MonoBehaviour
    {
        [SerializeField] private Volume postProcessing;
        [Range(0, 1)][SerializeField] private float redThreshold = .0f;
        [Range(0, 1)][SerializeField] private float greenThreshold = .0f;
        [Range(0, 1)][SerializeField] private float blueThreshold = .0f;

        private UnityEngine.Rendering.Universal.ColorCurves _colorCurves;
        
        private CharacterController _xrChara = null;
        private Vector3 _lastPos;
        private Quaternion _lastRot;
        private Vector3 _lastAngularVelocity;
        private int _changing;
        private Coroutine _changeColourRoutine;

        private Keyframe[] keyFrames;
        private TextureCurveParameter redTex;
        private TextureCurveParameter greenTex;
        private TextureCurveParameter blueTex;

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
            keyFrames = new Keyframe[3];
            keyFrames[0] = new Keyframe(redThreshold, redThreshold * 3 / 4);
            keyFrames[1] = new Keyframe(greenThreshold, greenThreshold * 3 / 4);
            keyFrames[2] = new Keyframe(blueThreshold, blueThreshold * 3 / 4);
        }

        // Update is called once per frame
        void Update()
        {
            if ( GameHandler.State == GameHandler.StateType.Playing)
            {
                var m = false;
                var t = false;
                var v = _xrChara.velocity.magnitude;
                Vector3 pos = _xrChara.transform.position;
                // If moving
                //velocity sometimes stick to a value even when stopped
                if (v > 0.3 && pos != _lastPos)
                {
                    m = true;
                }

                _lastPos = pos;

                Quaternion rot = GameHandler.Instance.XROrigin.transform.rotation;
                Quaternion deltaRot = rot * Quaternion.Inverse(_lastRot);
                var eulerRot = new Vector3(Mathf.DeltaAngle(0, deltaRot.eulerAngles.x), Mathf.DeltaAngle(0, deltaRot.eulerAngles.y), Mathf.DeltaAngle(0, deltaRot.eulerAngles.z));
                Vector3 angularVelocity = eulerRot / Time.fixedDeltaTime;

                // If turning
                if (_lastAngularVelocity.magnitude > 10 && angularVelocity.magnitude > 10)
                {
                    t = true;
                }

                _lastAngularVelocity = angularVelocity;
                _lastRot = rot;
                if (_changing != (m || t ? 1 : -1))
                {
                    if (_changeColourRoutine != null) StopCoroutine(_changeColourRoutine);
                    _changeColourRoutine = StartCoroutine(ChangeColour(m || t));
                }

            }
        }

        private IEnumerator ChangeColour(bool moving)
        {
            if (moving)
            {
                redTex.value.MoveKey(1, keyFrames[0]);
                redTex.value.AddKey(1, 1);

                greenTex.value.MoveKey(1, keyFrames[1]);
                greenTex.value.AddKey(1, 1);

                blueTex.value.MoveKey(1, keyFrames[2]);
                blueTex.value.AddKey(1, 1);
            } else
            {
                //greenTex.Release();?
            }
            Debug.Log(greenTex.value.length);
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

