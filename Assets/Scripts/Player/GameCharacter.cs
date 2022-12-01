using System;
using Options.Gameplay;
using UnityEngine;

namespace Player
{
    public class GameCharacter : MonoBehaviour
    {
        private Terrain _activeTerrain;
        private Vector3 _spawnPos = Vector3.zero;
        
        public enum SpawnPoint
        {
            AtSpawn,
            AtPosition,
            AtPath
        }
        private void Start()
        {
            _activeTerrain = Terrain.activeTerrain;
            Respawn(SpawnPoint.AtPosition);
            _spawnPos = transform.position;
            GameManager.Instance.LastCollectiblePos = _spawnPos;

        }

        public void Respawn(SpawnPoint atSpawn = SpawnPoint.AtSpawn)
        {
            if (_activeTerrain != null)
            {
                Vector3 pos;
                switch (atSpawn)
                {
                    case SpawnPoint.AtPath:
                        pos = GameManager.Instance.LastCollectiblePos;
                        break;
                    case SpawnPoint.AtPosition:
                        pos = transform.position;
                        break;
                    case SpawnPoint.AtSpawn:
                        pos = _spawnPos;
                        break;
                    default:
                        pos = Vector3.zero;
                        break;
                }
                pos.y = _activeTerrain.SampleHeight(pos) + _activeTerrain.transform.position.y + 0.2f;
                transform.position = pos;
            }
        }
    }
}