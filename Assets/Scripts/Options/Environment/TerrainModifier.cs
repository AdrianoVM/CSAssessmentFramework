using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Options.Environment
{
    [ExecuteInEditMode]
    public class TerrainModifier : MonoBehaviour
    {
        
        [Header("Repeating Terrain Shape")]
        [SerializeField] private float sizeOfCurve = 5f;
        [SerializeField] private bool curveApplyToX = true;
        [SerializeField] private bool curveApplyToZ = true;
        [SerializeField] private AnimationCurve smallScaleCurve;
        [SerializeField] private bool usePerlinNoise;
        [SerializeField] private float perlinScale = 10;
        [SerializeField] private float perlinIntensity = 10;
        [Header("Terrain Orientation")]
        [Range(-20,20)]
        [SerializeField] private int angle;

        [SerializeField] private bool angleApplyToZInsteadOfX;

        [SerializeField] private Transform rotateWithTerrain;

        private int _tRes;

        private Terrain _terrain;
        public Terrain TargetTerrain => _terrain;

        private void OnEnable()
        {
            _terrain = Terrain.activeTerrain;
            _tRes = TargetTerrain.terrainData.heightmapResolution;
        }


        public void Apply()
        {
            float oriX = Random.Range(0, 999);
            float oriZ = Random.Range(0, 999);
            var tMaxHeight = TargetTerrain.terrainData.heightmapScale.y;
            Transform transformT = TargetTerrain.transform;
            Vector3 position = transformT.position;
            position = new Vector3(position.x, -tMaxHeight/2, position.z);
            transformT.position = position;
            var arr = new float[_tRes, _tRes];
            for (var k = 0; k < _tRes; k+=1)
            {
                for (var l = 0; l < _tRes; l+=1)
                {
                    TerrainData terrainData = TargetTerrain.terrainData;
                    var xPos = k * terrainData.size.x / _tRes;
                    var zPos = l * terrainData.size.z / _tRes;

                    //Orienting terrain
                    float orientation;
                    if (angleApplyToZInsteadOfX)
                    {
                        orientation = (zPos - terrainData.size.z / 2f) *Mathf.Sin(angle * Mathf.Deg2Rad)/Mathf.Sin((90-angle) * Mathf.Deg2Rad);
                    }
                    else
                    {
                        orientation = (xPos - terrainData.size.x / 2f) * Mathf.Sin(-angle * Mathf.Deg2Rad)/Mathf.Sin((90-angle) * Mathf.Deg2Rad);
                    }
                    
                    
                    //applying the small scale modification
                    var h = (curveApplyToZ ? smallScaleCurve.Evaluate(xPos% (sizeOfCurve+1) / sizeOfCurve) : 1) 
                            * (curveApplyToX ? smallScaleCurve.Evaluate(zPos% (sizeOfCurve+1) / sizeOfCurve) : 1);
                    if (!curveApplyToX && !curveApplyToZ)
                    {
                        h = 0;
                    }

                    if (usePerlinNoise)
                    {
                        
                        var xP = xPos / perlinScale + oriX;
                        var zP = zPos / perlinScale + oriZ;
                        var sample = Mathf.PerlinNoise(xP, zP);
                        h += (sample - 0.5f)*perlinIntensity;
                    }
                    
                    arr[k, l] = (h+orientation+tMaxHeight/2)/tMaxHeight;
                }
                    
            }
            TargetTerrain.terrainData.SetHeights(0,0,arr);
            
            if (rotateWithTerrain != null)
            {
                rotateWithTerrain.eulerAngles = angleApplyToZInsteadOfX ? new Vector3(0,0,angle) : new Vector3(angle,0,0);
            }
        }
    }
}