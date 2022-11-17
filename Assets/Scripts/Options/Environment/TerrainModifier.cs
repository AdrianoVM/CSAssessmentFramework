using System;
using UnityEngine;

namespace Options.Environment
{
    [ExecuteInEditMode]
    public class TerrainModifier : MonoBehaviour
    {
        private Terrain _terrain;
        [Header("Repeating Terrain Shape")]
        [SerializeField] private float sizeOfCurve = 5f;
        [SerializeField] private bool curveApplyToX = true;
        [SerializeField] private bool curveApplyToY = true;
        [SerializeField] private AnimationCurve smallScaleCurve;
        [Header("Terrain Orientation")]
        [Range(-30,30)]
        [SerializeField] private int zAngle;
        [Range(-30,30)]
        [SerializeField] private int xAngle;

        private int _xRes;
        private int _yRes;
        
        private void Start()
        {
            _terrain = Terrain.activeTerrain;
            _xRes = _terrain.terrainData.heightmapResolution;
        }

        public void Apply()
        {
            var tMaxHeight = _terrain.terrainData.heightmapScale.y;
            Transform transformT = _terrain.transform;
            Vector3 position = transformT.position;
            position = new Vector3(position.x, -tMaxHeight/2, position.z);
            transformT.position = position;
            var arr = new float[_xRes, _xRes];
            for (var k = 0; k < arr.GetLength(0); k+=1)
            {
                for (var l = 0; l < arr.GetLength(1); l+=1)
                {
                    var zOrientation = (k - arr.GetLength(0) / 2)*Mathf.Sin(zAngle* Mathf.Deg2Rad);
                    var xOrientation = (l - arr.GetLength(1) / 2)*Mathf.Sin(xAngle* Mathf.Deg2Rad);
                    
                    var h = (curveApplyToY ? smallScaleCurve.Evaluate(k% (sizeOfCurve+1) / sizeOfCurve) : 1) 
                            * (curveApplyToX ? smallScaleCurve.Evaluate((l% (sizeOfCurve+1)) / sizeOfCurve) : 1);
                    arr[k, l] = (h+zOrientation+xOrientation+tMaxHeight/2)/tMaxHeight;
                }
                    
            }
            
            
            _terrain.terrainData.SetHeights(0,0,arr);
        }
    }
}