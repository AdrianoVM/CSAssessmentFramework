using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Options.Gameplay;

namespace Options.Vision
{
    [ExecuteInEditMode]
    public class ColorBlur : MonoBehaviour
    {
        [SerializeField] private Volume postProcessing;

        private UnityEngine.Rendering.Universal.ColorCurves _colorCurves;
        private CharacterController _xrChara; // do I need this?

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
        }

        // Start is called before the first frame update
        void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            _xrChara = GameHandler.Instance.XROrigin.GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDisable()
        {
            if (postProcessing != null && _colorCurves != null)
            {
                //_depthOfField.intensity.value = _vignette.intensity.min;
                _colorCurves.active = false;
            }
        }
    }
}

