using System;
using Options.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class GameManagerUI : MonoBehaviour
    {
        [SerializeField] private Button startExperimentButton;
        [SerializeField] private Button endExperimentButton;
        [SerializeField] private TextMeshProUGUI timeLabel;

        [SerializeField] private TMP_InputField timeInput;

        [SerializeField] private TextMeshProUGUI collectiblesLabel;
        [SerializeField] private TMP_InputField collectiblesInput;
        
        private GameManager _gM;


        private void OnEnable()
        {
            
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            
        }

        private void Start()
        {
            _gM = GameManager.Instance;
            endExperimentButton.gameObject.SetActive(false);
            if (timeLabel != null && timeInput != null)
            {
                var t = _gM.ExperimentLength > 0 ? _gM.ExperimentLength.ToString() : "∞";
                timeInput.text = t;
                timeLabel.text = "0.0 / ";

                var c = _gM.NumberOfCollectiblesToPickUp > 0 ? _gM.NumberOfCollectiblesToPickUp.ToString() : "∞";
                collectiblesInput.text = c;
                collectiblesLabel.text = "0 / ";
            }
        }

        private void Update()
        {
            if (GameManager.State == GameManager.StateType.Playing)
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

        private void GameManagerOnGameStateChanged(GameManager.StateType state)
        {
            switch (state)
            {
                case GameManager.StateType.Menu:
                    startExperimentButton.gameObject.SetActive(true);
                    endExperimentButton.gameObject.SetActive(false);
                    timeInput.interactable = true;
                    collectiblesInput.interactable = true;
                    timeLabel.text = (_gM != null ? _gM.PlayTime.ToString("0.0") : 0) + " / ";
                    collectiblesLabel.text = (_gM != null ? _gM.PickedUpCollectibles.ToString() : 0) + " / ";
                    break;
                
                case GameManager.StateType.Playing:
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
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
        }
    }
}