using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Options.Gameplay;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Options.Vision
{
    /// <summary>
    /// Slightly Modified version of DynamicFOVFilter.cs from https://github.com/angsamuel/GingerVR
    /// </summary>
    public class DynamicFOVFilter : MonoBehaviour
    {
        
        public GameObject filter;

        public float decaySpeed = 20f; 

        public float minFilterScale = .5f;
        public float maxFilterScale = 1.5f;
        

        public float decayRate = -.001f;
        public float angluarSpeedModifier = .1f;
        public float translationSpeedModifier = .1f;
        

        Vector3 angularVelocity;
        Vector3 translationalVelocity;

        
        float speed = 0;
        float angularSpeed = 0;
        float filterScale = 0f;


        float checkTime = .1f;

        private IEnumerator TrackVelocities(){
            Vector3 lastRotation = new Vector3(0,0,0);
            Vector3 lastPosition = transform.position;
            while(true)
            {
                Vector3 rotationDelta = transform.eulerAngles - lastRotation;
                rotationDelta = rotationDelta/30f; // to smooth it.
                lastRotation = lastRotation + rotationDelta;

                Vector3 translationDelta = transform.position - lastPosition;
                if (Vector3.Scale(translationDelta/ checkTime, transform.localScale.Inverse()).magnitude > 5)
                {
                    translationDelta = Vector3.zero;
                    lastPosition = transform.position;
                }
                else
                {
                    translationDelta /= 10f;
                    lastPosition = lastPosition + translationDelta;
                }
                

                angularVelocity = rotationDelta / checkTime;
                translationalVelocity = translationDelta / checkTime;

                yield return new WaitForSeconds(checkTime);
            }
        }


        private void Start()
        {
            StartCoroutine(TrackVelocities());
        }



        

        void Update()
        {
            float translationalSpeed;
            float rotationalSpeed;
            translationalSpeed = translationalVelocity.magnitude;
            rotationalSpeed = angularVelocity.magnitude;
            float cRate = ((translationalSpeed - decaySpeed) * translationSpeedModifier) + (rotationalSpeed*angluarSpeedModifier);


            
            if(translationalSpeed <= decaySpeed){
                filterScale -= decayRate;
            }else{
                filterScale -= Mathf.Abs(cRate);
            }
            if(filterScale > maxFilterScale){
                filterScale = maxFilterScale;  
            }
            if(filterScale < minFilterScale){
                filterScale = minFilterScale;
            }


            //update scale of filter
            filter.transform.localScale = new Vector3(filterScale,filterScale,1);

        }
    }
}