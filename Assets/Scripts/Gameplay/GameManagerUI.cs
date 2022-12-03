using System;
using Options.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class GameManagerUI : MonoBehaviour
    {
        [SerializeField] private Button startExperimentButton;
        [SerializeField] private Button endExperimentButton;


        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            endExperimentButton.gameObject.SetActive(false);
        }

        private void GameManagerOnGameStateChanged(GameManager.StateType state)
        {
            switch (state)
            {
                case GameManager.StateType.Menu:
                    startExperimentButton.gameObject.SetActive(true);
                    endExperimentButton.gameObject.SetActive(false);
                    break;
                case GameManager.StateType.Playing:
                    startExperimentButton.gameObject.SetActive(false);
                    endExperimentButton.gameObject.SetActive(true);
                    break;
            }
        }


        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
        }
    }
}