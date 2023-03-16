using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Options.Vision
{
    [ExecuteInEditMode]
    public class RestFrames : MonoBehaviour
    {
        [Header("Single Nose")]
        [SerializeField] private bool singleNose;
        [SerializeField] private GameObject singleNosePrefab;
        [Range(0, 1)][SerializeField] private float yPosition = .5f;
        [Range(0, 1)][SerializeField] private float zPosition = .5f;
        [Range(0f, 1f)][SerializeField] private float noseWidth = 1;
        [Range(0f, 1f)][SerializeField] private float noseFlatness = 1;
        


        void Start()
        {
            // SINGLE NOSE --------------------------------------------
            if (singleNose && singleNosePrefab == null)
            {
                // try to find the nose prefab if it is null
                singleNosePrefab = GameObject.Find("SingleNose");
                if (singleNosePrefab == null) {
                    Debug.Log("Single Nose Prefab is not found on the scene.");
                    singleNose = false;
                }
            } else if ( singleNosePrefab != null) {
                singleNosePrefab.SetActive(singleNose);
                var noseScript = singleNosePrefab.GetComponent<SingleNose>();
                noseScript.YPosition = yPosition;
                noseScript.ZPosition = zPosition;
                noseScript.NoseWidth = noseWidth;
                noseScript.NoseFlatness = noseFlatness;
            }

        }


        void Update()
        {
            
            
        }
    }
}

