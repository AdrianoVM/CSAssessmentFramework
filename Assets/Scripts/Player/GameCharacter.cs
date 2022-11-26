using System;
using UnityEngine;

namespace Player
{
    public class GameCharacter : MonoBehaviour
    {
        private Terrain _activeTerrain;
        private Vector3 _spawnPos = Vector3.zero;
        private void Start()
        {
            _activeTerrain = Terrain.activeTerrain;
            Respawn(false);
            _spawnPos = transform.position;

        }

        public void Respawn(bool atSpawn=true)
        {
            if (_activeTerrain != null)
            {
                Vector3 pos = atSpawn ? _spawnPos : transform.position;
                pos.y = _activeTerrain.SampleHeight(pos) + _activeTerrain.transform.position.y + 0.2f;
                transform.position = pos;
            }
        }
    }
}