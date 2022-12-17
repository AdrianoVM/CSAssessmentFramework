using UnityEngine;

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
        [Header("Perlin Noise")]
        [SerializeField] private bool usePerlinNoise;
        [Range(1,100)]
        [SerializeField] private int perlinSeed;
        [Range(5,60)]
        [SerializeField] private int perlinScale = 10;
        [Range(0,30)]
        [SerializeField] private int perlinIntensity = 10;
        [Header("Terrain Orientation")]
        [Range(-20,20)]
        [SerializeField] private int angle;

        [SerializeField] private bool angleApplyToZNotX;

        [SerializeField] private Transform rotateWithTerrain;

        private int _tRes;

        private Terrain _terrain;
        public Terrain TargetTerrain => _terrain;

        private void OnEnable()
        {
            _terrain = Terrain.activeTerrain;
            _tRes = TargetTerrain.terrainData.heightmapResolution;
        }


        /// <summary>
        /// Applies the terrain modifications to the terrain, and rotates <see cref="rotateWithTerrain"/> to correspond.
        /// </summary>
        public void Apply()
        {
            var tMaxHeight = TargetTerrain.terrainData.heightmapScale.y;
            Transform transformT = TargetTerrain.transform;
            Vector3 position = transformT.position;
            position = new Vector3(position.x, -tMaxHeight/2, position.z);
            transformT.position = position;
            var arr = new float[_tRes, _tRes];
            // At each point of the terrain.
            for (var k = 0; k < _tRes; k+=1)
            {
                for (var l = 0; l < _tRes; l+=1)
                {
                    TerrainData terrainData = TargetTerrain.terrainData;
                    var xPos = k * terrainData.size.x / _tRes;
                    var zPos = l * terrainData.size.z / _tRes;

                    //Orienting terrain
                    float orientation;
                    if (angleApplyToZNotX)
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
                        // Scaling down effect near 0,0 (spawn)
                        var perlinBlendX = Mathf.Clamp01(Mathf.Abs(xPos - terrainData.size.x / 2f) / 20f);
                        var perlinBlendZ = Mathf.Clamp01(Mathf.Abs(zPos - terrainData.size.z / 2f) / 20f);
                        var perlinBlend = new Vector2(perlinBlendX, perlinBlendZ).magnitude / Mathf.Sqrt(2);

                        var xP = xPos / perlinScale + perlinSeed;
                        var zP = zPos / perlinScale + perlinSeed;
                        var sample = Mathf.Clamp01(Mathf.PerlinNoise(xP, zP));
                        h += (sample - 0.5f)*perlinIntensity*perlinBlend;
                    }
                    
                    arr[k, l] = (h+orientation+tMaxHeight/2)/tMaxHeight;
                }
                    
            }
            TargetTerrain.terrainData.SetHeights(0,0,arr);
            
            if (rotateWithTerrain != null)
            {
                rotateWithTerrain.eulerAngles = angleApplyToZNotX ? new Vector3(0,0,angle) : new Vector3(angle,0,0);
            }
        }
    }
}