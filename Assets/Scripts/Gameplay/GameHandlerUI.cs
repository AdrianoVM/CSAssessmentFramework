using System;
using System.Linq;
using DataSaving;
using Options.Gameplay;
using Options.Movement;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gameplay
{
    public class GameHandlerUI : MonoBehaviour
    {
        [SerializeField] private Button startExperimentButton;
        [SerializeField] private Button endExperimentButton;
        [SerializeField] private TextMeshProUGUI timeLabel;

        [SerializeField] private TMP_InputField timeInput;

        [SerializeField] private TextMeshProUGUI collectiblesLabel;
        [SerializeField] private TMP_InputField collectiblesInput;

        [Header("Locomotion options")] 
        [SerializeField] private GameObject locomotionHolder;
        [SerializeField] private TMP_Dropdown leftLocDropdown;
        [SerializeField] private TMP_Dropdown rightLocDropdown;
        [SerializeField] private TMP_Dropdown leftTurnDropdown;
        [SerializeField] private TMP_Dropdown rightTurnDropdown;
        [SerializeField] private Toggle leftHandGrabToggle;
        [SerializeField] private Toggle rightHandGrabToggle;
        
        [Header("Data Saving")]
        [SerializeField] private GameObject savingHolder;
        [SerializeField] private TextMeshProUGUI fileNameLabel;
        [SerializeField] private TextMeshProUGUI savedNotificationLabel;
        
        private GameHandler _gM;
        private LocomotionHandler _locomotionHandler;


        private void OnEnable()
        {
            
            GameHandler.GameStateChanged += GameManagerOnGameStateChanged;
            // no unsubscribing with anonymous functions
            DataSaver.FolderChanged += (v) => fileNameLabel.text = v;
            DataSaver.DataSaved += (v) => savedNotificationLabel.gameObject.SetActive(v);
        }

        private void Start()
        {
            _gM = GameHandler.Instance;
            _locomotionHandler = _gM.XROrigin.GetComponent<LocomotionHandler>();
            endExperimentButton.gameObject.SetActive(false);
            savedNotificationLabel.gameObject.SetActive(false);
            if (timeLabel != null && timeInput != null)
            {
                var t = _gM.ExperimentLength > 0 ? _gM.ExperimentLength.ToString() : "∞";
                timeInput.text = t;
                timeLabel.text = "0.0 / ";

                var c = _gM.NumberOfCollectiblesToPickUp > 0 ? _gM.NumberOfCollectiblesToPickUp.ToString() : "∞";
                collectiblesInput.text = c;
                collectiblesLabel.text = "0 / ";
            }

            if (leftLocDropdown != null && rightLocDropdown != null && leftTurnDropdown != null && rightTurnDropdown != null)
            {
                var movementTypesNames = Enum.GetNames(typeof(LocomotionHandler.MovementType))
                    .Select(i => new TMP_Dropdown.OptionData(i)).ToList();
                leftLocDropdown.options = movementTypesNames;
                leftLocDropdown.value = (int)_locomotionHandler.LeftHandLocomotionType;
                rightLocDropdown.options = movementTypesNames;
                rightLocDropdown.value = (int)_locomotionHandler.RightHandLocomotionType;
                leftTurnDropdown.options = movementTypesNames;
                leftTurnDropdown.value = (int)_locomotionHandler.LeftHandTurnType;
                rightTurnDropdown.options = movementTypesNames;
                rightTurnDropdown.value = (int)_locomotionHandler.RightHandTurnType;
            }

            if (leftHandGrabToggle != null && rightHandGrabToggle != null)
            {
                leftHandGrabToggle.isOn = _locomotionHandler.LeftHandGrabMove;
                rightHandGrabToggle.isOn = _locomotionHandler.RightHandGrabMove;
            }
            
        }

        private void Update()
        {
            if (GameHandler.State == GameHandler.StateType.Playing)
            {
                if (timeLabel != null)
                {
                    
                    timeLabel.text = _gM.PlayTime.ToString("0.0") + " / ";
                }
                if (collectiblesLabel != null)
                {
                    collectiblesLabel.text = _gM.PickedUpCollectibles.ToString() + " / ";
                }
            }
            
        }

        public void CollectiblesInputChange(string input)
        {
            int.TryParse(input, out var i);
            _gM.NumberOfCollectiblesToPickUp = i;
        }
        
        public void TimerInputChange(string input)
        {
            int.TryParse(input, out var i);
            _gM.ExperimentLength = i;
        }

        private void GameManagerOnGameStateChanged(GameHandler.StateType state)
        {
            switch (state)
            {
                case GameHandler.StateType.Menu:
                    savingHolder.SetActive(true);
                    locomotionHolder.SetActive(true);
                    startExperimentButton.gameObject.SetActive(true);
                    endExperimentButton.gameObject.SetActive(false);
                    timeInput.interactable = true;
                    collectiblesInput.interactable = true;
                    timeLabel.text = (_gM != null ? _gM.PlayTime.ToString("0.0") : 0) + " / ";
                    collectiblesLabel.text = (_gM != null ? _gM.PickedUpCollectibles.ToString() : 0) + " / ";
                    break;
                
                case GameHandler.StateType.Playing:
                    savingHolder.SetActive(false);
                    locomotionHolder.SetActive(false);
                    startExperimentButton.gameObject.SetActive(false);
                    endExperimentButton.gameObject.SetActive(true);
                    timeInput.interactable = false;
                    var t = _gM.ExperimentLength > 0 ? _gM.ExperimentLength.ToString() : "∞";
                    timeInput.text = t;
                    collectiblesInput.interactable = false;
                    var c = _gM.NumberOfCollectiblesToPickUp > 0 ? _gM.NumberOfCollectiblesToPickUp.ToString() : "∞";
                    collectiblesInput.text = c;
                    
                    break;
            }
        }


        private void OnDisable()
        {
            GameHandler.GameStateChanged -= GameManagerOnGameStateChanged;
        }
    }
}