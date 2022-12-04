using System;
using System.Collections;
using Options.Gameplay;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Player
{
    public class GameCharacter : MonoBehaviour
    {
        
        [SerializeField] private Volume darkVolume;

        public enum SpawnPoint
        {
            AtSpawn,
            AtPosition,
            AtPath
        }

        private Terrain _activeTerrain;
        private Vector3 _spawnPos = Vector3.zero;
        private Vignette _vignette;


        private void OnEnable()
        {
            GameManager.GameStarted += OnGameStart;
            GameManager.GameEnded += OnGameEnd;
        }

        private void Start()
        {
            if (darkVolume == null)
            {
                Debug.LogWarning("No Post Processing set");
                return;
            }
            if (!darkVolume.profile.TryGet(out _vignette))
            {
                Debug.LogWarning("No vignette found");
                return;
            }
            _vignette.intensity.overrideState = true;
            _vignette.active = true;
            _vignette.intensity.value = 1;
            
            _activeTerrain = Terrain.activeTerrain;
            Respawn(SpawnPoint.AtPosition);
            _spawnPos = transform.position;
            GameManager.Instance.LastCollectiblePos = _spawnPos;
        }

        private void OnGameStart()
        {
            Respawn();
        }

        private void OnGameEnd()
        {
            StartCoroutine(ChangeVignette(true, 2));
        }

        public void Respawn(SpawnPoint atSpawn = SpawnPoint.AtSpawn)
        {
            if (GameManager.State == GameManager.StateType.Playing)
            {
                StartCoroutine(ChangeVignette(false,2));
            }
            
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


        private IEnumerator ChangeVignette(bool toBlack, float fadeSpeed)
        {
            if (_vignette == null)
            {
                yield break;
            }
            _vignette.active = true;
            var fadeTo = toBlack ? 1 : 0;
            float fadeAmount;
            while (Math.Abs(_vignette.intensity.value - fadeTo) > 0.05f)
            {
                if (toBlack)
                {
                    fadeAmount = Mathf.Min(1, _vignette.intensity.value + (1/fadeSpeed * Time.deltaTime));
                }
                else
                {
                    fadeAmount = Mathf.Max(0, _vignette.intensity.value - (1/fadeSpeed * Time.deltaTime));
                }
                _vignette.intensity.value = fadeAmount;
                yield return null;
            }

            _vignette.intensity.value = fadeTo;
            if (! toBlack)
            {
                _vignette.active = false;
            }
        }
        

        private void OnDisable()
        {
            GameManager.GameStarted -= OnGameStart;
            GameManager.GameEnded -= OnGameEnd;
        }
    }
}