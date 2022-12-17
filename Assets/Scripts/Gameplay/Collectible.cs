using System;
using System.Collections;
using ScriptableObjects;
using UnityEngine;
using Utilities;

namespace Gameplay
{
    /// <summary>
    /// Floating collectible, when there is a collision with the player calls <see cref="OnPickedUp"/> and disappears.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Collectible : MonoBehaviour
    {
        [SerializeField] private CollectiblePickUpSO pickUpChannel;
        
        /// <summary>
        /// Material that is used when the Collectible can be picked up
        /// </summary>
        [SerializeField] private Material primaryMaterial;
        
        /// <summary>
        /// Material that is used when the Collectible cannot be picked up
        /// </summary>
        [SerializeField] private Material secondaryMaterial;

        [SerializeField] private float rotationSpeed = 10;
        [SerializeField] private float upDownPerSecond = 1;
        [SerializeField] private float upDownDistance = 0.5f;
        [SerializeField] private bool isInteractable;

        public event EventHandler OnPickedUp;
        

        private Collider _collider;
        private Renderer _renderer;
        private Vector3 _scale;
        private Terrain _activeTerrain;
        private Coroutine _hoverRoutine;


        private void Start()
        {
            _activeTerrain = Terrain.activeTerrain;
            _scale = transform.localScale;
            ReEnable();
        }

        private void OnEnable()
        {
            _collider = GetComponent<Collider>();
            _renderer = GetComponent<Renderer>();
            SetInteractable(isInteractable);
        }


        public void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
            if (_collider != null)
            {
                _collider.enabled = interactable;
            }

            if (primaryMaterial != null && secondaryMaterial != null)
            {
                _renderer.material = interactable ? primaryMaterial : secondaryMaterial;
            }
        }

        /// <summary>
        /// Set position based on Terrain, start respawn effect and the calls coroutine to hover.
        /// </summary>
        public void ReEnable()
        {
            //set position based on terrain
            if (_activeTerrain != null)
            {
                Vector3 pos = transform.position;
                pos.y = _activeTerrain.SampleHeight(pos) + _activeTerrain.transform.position.y + 1f;
                transform.position = pos;
            }

            StartCoroutine(RespawnEffect());
            
            _hoverRoutine = StartCoroutine(Hover());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SetInteractable(false);
                StopCoroutine(_hoverRoutine);
                StartCoroutine(PickedUp());
            }
        }

        private IEnumerator PickedUp()
        {
            //play sound
            SoundManager.PlaySound(SoundManager.Sound.CoinPickup, transform.position);
            //play Animation
            Vector3 startScale = _scale;
            var timer = 0.0f;

            while (timer < 0.5f)
            {
                timer += Time.deltaTime;
                var t = timer / 0.5f;
                //smoother step algorithm
                t = t * t * t * (t * (6f * t - 15f) + 10f);
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                yield return null;
            }

            //Raising events
            if (pickUpChannel != null)
            {
                pickUpChannel.RaiseEvent(this);
            }
            if (OnPickedUp != null) OnPickedUp(this, EventArgs.Empty);
            //Disappear
            yield return null;
        }

        private IEnumerator RespawnEffect()
        {
            var timer = 0.0f;
            while (timer < 1)
            {
                timer += Time.deltaTime;
                var t = timer / 1;
                //smoother step algorithm
                t = t * t * t * (t * (6f * t - 15f) + 10f);
                transform.localScale = Vector3.Lerp(Vector3.zero, _scale, t);
                yield return null;
            }
        }

        private IEnumerator Hover()
        {
            var start = Time.time;
            while (isActiveAndEnabled)
            {
                transform.position += Vector3.up * (Mathf.Cos((Time.time-start)*upDownPerSecond) * upDownDistance/2 * Time.deltaTime);
                transform.Rotate(Vector3.up, Time.deltaTime*rotationSpeed);
                yield return null;
            }
        }
    }
}