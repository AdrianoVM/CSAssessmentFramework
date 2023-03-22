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

        [Header("Double Nose")]
        [SerializeField] private bool doubleNose;
        [SerializeField] private GameObject doubleNosePrefab;

        [Header("Hat")]
        [SerializeField] private bool hat;
        [SerializeField] private GameObject hatPrefab;


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

            // DOUBLE NOSE --------------------------------------------
            if (doubleNose && doubleNosePrefab == null)
            {
                // try to find the nose prefab if it is null
                doubleNosePrefab = GameObject.Find("SingleNose");
                if (doubleNosePrefab == null)
                {
                    Debug.Log("Single Nose Prefab is not found on the scene.");
                    doubleNose = false;
                }
            }
            else if (doubleNosePrefab != null)
            {
                doubleNosePrefab.SetActive(doubleNose);
            }

            // HAT --------------------------------------------
            if (hat && hatPrefab == null)
            {
                // try to find the prefab if it is null
                hatPrefab = GameObject.Find("Sombrero");
                if (hatPrefab == null)
                {
                    Debug.Log("Hat Prefab is not found on the scene.");
                    hat = false;
                }
            }
            else if (hatPrefab != null)
            {
                hatPrefab.SetActive(hat);
            }
        }

    }
}

