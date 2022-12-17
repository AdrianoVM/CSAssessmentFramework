using System;
using System.Collections.Generic;
using Gameplay;
using PathCreation;
using UnityEngine;

namespace Options.Gameplay.Activity {

    
    public class CollectiblePlacer : MonoBehaviour
    {
        [SerializeField] private PathCreator pathCreator;
        [SerializeField] private Collectible collectible;
        [SerializeField] private GameObject holder;
        public float spacing = 5;
        public int numberOfVisibleCollectibles = 5;

        private const float MinSpacing = 1f;
        private float _currentDist;
        private Queue<Collectible> _collectibles = new();


        private void OnEnable()
        {
            GameHandler.GameStarted += OnGameStart;
        }

        private void OnGameStart()
        {
            CreatePath();
        }

        private void Start()
        {
            CreatePath();
        }

        /// <summary>
        /// Creates and places the first visible collectibles on path, populating <see cref="_collectibles"/>,
        /// and starting to listen each coin's <see cref="Collectible.OnPickedUp"/>.
        /// </summary>
        private void CreatePath()
        {
            // Clearing possible existing path
            foreach (Collectible col in _collectibles)
            {
                col.OnPickedUp -= OnCollectiblePickup;
                Destroy(col.gameObject);
            }
            _collectibles.Clear();
            
            spacing = Mathf.Max(MinSpacing, spacing);
            //place the visible coins
            if (pathCreator != null && collectible != null && holder != null)
            {
                for (var i = 0; i < numberOfVisibleCollectibles; i++)
                {
                    
                    Vector3 point = pathCreator.path.GetPointAtDistance(i*spacing);
                    Collectible instCol = Instantiate(collectible, point, Quaternion.identity, holder.transform);
                    if (i == 0)
                    {
                        instCol.SetInteractable(true);
                    }
                    else
                    {
                        instCol.SetInteractable(false);
                    }
                    instCol.OnPickedUp += OnCollectiblePickup;
                    _collectibles.Enqueue(instCol);
                }

                
                _currentDist = numberOfVisibleCollectibles*spacing;
            }
            else
            {
                Debug.LogWarning("Collectible Placement not possible as setup is not complete.");
            }
        }

        /// <summary>
        /// Places the <see cref="Collectible"/> <paramref name="sender"/> to next point in line.
        /// </summary>
        private void OnCollectiblePickup(object sender, EventArgs e)
        {
            Collectible pickedCollectible = _collectibles.Dequeue();
            
            Vector3 point = pathCreator.path.GetPointAtDistance(_currentDist);
            // Rotating is also possible, but not needed in our case
            //Quaternion rot = pathCreator.path.GetRotationAtDistance(_currentDist);
            Transform collectibleTransform = pickedCollectible.transform;
            collectibleTransform.position = point;
            //collectibleTransform.rotation = rot;
            
            pickedCollectible.ReEnable();
            _collectibles.Enqueue(pickedCollectible);
            
            _collectibles.Peek().SetInteractable(true);
            _currentDist += spacing;
        }

        private void OnDisable()
        {
            foreach (Collectible col in _collectibles)
            {
                col.OnPickedUp -= OnCollectiblePickup;
            }

            GameHandler.GameStarted -= OnGameStart;
        }
    }
}