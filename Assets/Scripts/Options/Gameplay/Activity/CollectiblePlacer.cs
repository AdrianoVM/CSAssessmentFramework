using System;
using System.Collections.Generic;
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
        private float _currentDist = 0;
        private Queue<Collectible> _collectibles = new();


        private void OnEnable()
        {
            GameManager.GameStarted += OnGameStart;
        }

        private void OnGameStart()
        {
            CreatePath();
        }

        private void Start()
        {
            CreatePath();
        }

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
            if (pathCreator != null && collectible != null && holder != null)
            {
                for (var i = 0; i < numberOfVisibleCollectibles; i++)
                {
                    
                    Vector3 point = pathCreator.path.GetPointAtDistance(i*spacing);
                    //Quaternion rot = pathCreator.path.GetRotationAtDistance(i*spacing);
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

        private void OnCollectiblePickup(object sender, EventArgs e)
        {
            Collectible pickedCollectible = _collectibles.Dequeue();
            
            Vector3 point = pathCreator.path.GetPointAtDistance(_currentDist);
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

            GameManager.GameStarted -= OnGameStart;
        }
    }
}