using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//double nose -.19, -.16, .45
//scale .1

//single nose z: .4 - .8
//single nose 
// light skin color: E8BEAC, (232, 190, 172)

public class SingleNose : MonoBehaviour
{
    [SerializeField] private float yPosition;
    [SerializeField] private float zPosition;
    [SerializeField] private float noseWidth;
    [SerializeField] private float noseFlatness;

    [SerializeField] public float YPosition { get { return yPosition; } set { yPosition = value; } }
    [SerializeField] public float ZPosition { get { return zPosition; } set { zPosition = value; } }
    [SerializeField] public float NoseWidth { get { return noseWidth; } set { noseWidth = value; } }
    [SerializeField] public float NoseFlatness { get { return noseFlatness; } set { noseFlatness = value; } }

    void Start()
    {
        float zPos = Mathf.Lerp(0.4f, 0.8f, zPosition);
        float yPos = Mathf.Lerp(-0.5f, 0.5f, yPosition);
        float xScale = Mathf.Lerp(0.05f, .15f, noseWidth);
        float yScale = Mathf.Lerp(0.05f, .25f, 1 - noseFlatness);
        float zScale = Mathf.Lerp(.03f, .15f, .5f);

        transform.localScale = new Vector3(xScale, yScale, zScale);
        transform.localPosition = new Vector3(transform.localPosition.x, yPos, zPos);
    }

}
