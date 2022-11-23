using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Options.Environment
{
    [ExecuteInEditMode]
    public class TerrainModifier : MonoBehaviour
    {
        private Terrain _terrain;
        [Header("Repeating Terrain Shape")]
        [SerializeField] private float sizeOfCurve = 5f;
        [SerializeField] private bool curveApplyToX = true;
        [SerializeField] private bool curveApplyToZ = true;
        [SerializeField] private AnimationCurve smallScaleCurve;
        [SerializeField] private bool usePerlinNoise;
        [SerializeField] private float perlinScale = 10;
        [SerializeField] private float perlinIntensity = 10;
        [Header("Terrain Orientation")]
        [Range(-30,30)]
        [SerializeField] private int zAngle;
        [Range(-30,30)]
        [SerializeField] private int xAngle;

        [SerializeField] private Transform rotateWithTerrain;

        private int _tRes;
        
        private void OnEnable()
        {
            _terrain = Terrain.activeTerrain;
            _tRes = _terrain.terrainData.heightmapResolution;
        }

        public void Apply()
        {
            float oriX = Random.Range(0, 999);
            float oriZ = Random.Range(0, 999);
            var tMaxHeight = _terrain.terrainData.heightmapScale.y;
            Transform transformT = _terrain.transform;
            Vector3 position = transformT.position;
            position = new Vector3(position.x, -tMaxHeight/2, position.z);
            transformT.position = position;
            var arr = new float[_tRes, _tRes];
            for (var k = 0; k < _tRes; k+=1)
            {
                for (var l = 0; l < _tRes; l+=1)
                {
                    TerrainData terrainData = _terrain.terrainData;
                    var xPos = k * terrainData.size.x / _tRes;
                    var zPos = l * terrainData.size.z / _tRes;

                    //Orienting terrain
                    var zOrientation = (xPos - terrainData.size.x / 2f) * Mathf.Sin(-xAngle * Mathf.Deg2Rad)/Mathf.Sin((90-xAngle) * Mathf.Deg2Rad);
                    var xOrientation = (zPos - terrainData.size.z / 2f) *Mathf.Sin(zAngle * Mathf.Deg2Rad)/Mathf.Sin((90-zAngle) * Mathf.Deg2Rad);;
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
                    
                    arr[k, l] = (h+zOrientation+xOrientation+tMaxHeight/2)/tMaxHeight;
                }
                    
            }
            _terrain.terrainData.SetHeights(0,0,arr);
            
            if (rotateWithTerrain != null)
            {
                rotateWithTerrain.eulerAngles = new Vector3(xAngle,0,zAngle);
            }
        }
    }
}