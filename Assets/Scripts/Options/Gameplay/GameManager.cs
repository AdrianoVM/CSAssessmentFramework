using System;
using Options.Gameplay.Activity;
using ScriptableObjects;
using UnityEngine;

namespace Options.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private CollectiblePickUpSO collectibleEventChannel;
        [SerializeField] private int numberOfCollectiblesToPickUp = 10;
        [SerializeField] private int gameLengthInSeconds;

        private int _pickedUpCollectibles = 0;

        private void OnEnable()
        {
            if (collectibleEventChannel)
            {
                collectibleEventChannel.OnCollectiblePickup += IncreaseCount;
            }
        }

        private void IncreaseCount(Collectible collectible)
        {
            _pickedUpCollectibles++;
            Debug.Log("Collected: " + collectible.name + " at "+collectible.transform.position);
            Debug.Log("nb of collectibles picked up: "+_pickedUpCollectibles);
            if (_pickedUpCollectibles >= numberOfCollectiblesToPickUp)
            {
                Debug.Log("Finished!");
            }
        }
        
        private void OnDisable()
        {
            if (collectibleEventChannel)
            {
                collectibleEventChannel.OnCollectiblePickup -= IncreaseCount;
            }
        }
    }
}