using System;
using Options.Gameplay.Activity;
using ScriptableObjects;
using UnityEngine;
using Utilities;

namespace Options.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        [SerializeField] private CollectiblePickUpSO collectibleEventChannel;
        [SerializeField] private int numberOfCollectiblesToPickUp = 10;
        [SerializeField] private int gameLengthInSeconds;
        [SerializeField] private bool playFMSPrompt;
        [SerializeField] private float FMSPromptInterval = 20f;

        private int _pickedUpCollectibles = 0;
        private bool _gameStarted;
        
        public enum StateType
        {
            Playing,
            Menu,
            Pause
        }

        public static StateType State;

        public Vector3 LastCollectiblePos { get; set; } = Vector3.zero;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            SoundManager.Initialize();
        }

        private void OnEnable()
        {
            if (collectibleEventChannel)
            {
                collectibleEventChannel.OnCollectiblePickup += IncreaseCount;
            }

            _gameStarted = true;
        }


        private void Update()
        {
            if (_gameStarted)
            {
                SoundManager.PlaySound(SoundManager.Sound.FMS);
            }
            
        }


        private void IncreaseCount(Collectible collectible)
        {
            _pickedUpCollectibles++;
            LastCollectiblePos = collectible.transform.position;
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