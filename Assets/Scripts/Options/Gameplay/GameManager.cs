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
        [SerializeField] private float FMSPromptInterval = 30f;

        private int _pickedUpCollectibles;
        private float _lastTimeFMSPlayed;
        private float _playTime;

        [Serializable]
        public enum StateType
        {
            Playing,
            Menu,
            Pause
        }

        private static StateType _state;
        
        public static StateType State
        {
            get => _state;
            private set
            {
                //This way when the event is called _state is still previous value
                if (GameStateChanged != null) GameStateChanged(value);
                _state = value;
            }
        }

        public static event Action<StateType> GameStateChanged;


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
            
            State = StateType.Menu;
        }


        private void Update()
        {
            if (State == StateType.Playing)
            {
                _playTime += Time.deltaTime;
                if (playFMSPrompt)
                {
                    if (_lastTimeFMSPlayed + FMSPromptInterval < _playTime)
                    {
                        _lastTimeFMSPlayed = _playTime;
                        SoundManager.PlaySound(SoundManager.Sound.FMS);
                    }
                }
            }
            
            
        }
        
        
        public void StartExperiment()
        {
            _pickedUpCollectibles = 0;
            State = StateType.Playing;
        }

        public void EndExperiment()
        {
            State = StateType.Menu;
        }

        public void PauseExperiment(bool pause)
        {
            State = pause ? StateType.Pause : StateType.Playing;
        }


        private void IncreaseCount(Collectible collectible)
        {
            _pickedUpCollectibles++;
            LastCollectiblePos = collectible.transform.position;
            Debug.Log("Collected: " + collectible.name + " at "+collectible.transform.position);
            Debug.Log("nb of collectibles picked up: "+_pickedUpCollectibles);
            if (_pickedUpCollectibles >= numberOfCollectiblesToPickUp)
            {
                State = StateType.Menu;
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