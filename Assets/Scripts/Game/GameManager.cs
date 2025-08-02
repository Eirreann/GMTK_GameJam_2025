using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private bool _isPersistent;
        
        public List<LevelManager> Levels;
        public LevelManager CurrentLevel => Levels[_levelIndex];
        public PlayerController Player;
        [HideInInspector] public InputHandler inputHandler;
        
        [SerializeField] private GameHUD _gameHUD;

        [SerializeField] private int _levelIndex = 0;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void Awake()
        {
            inputHandler = GetComponent<InputHandler>();
            
            Init(_isPersistent);
        }

        private void Start()
        {
            if (Levels.Count > 0)
                Levels[_levelIndex].StartLevel();
        }

        private void Update()
        {
            if(inputHandler._reset)
                ResetLevel();
        }

        public void ResetLevel()
        {
            CurrentLevel.Reset();
            Player.Reset();
        }

        public void ProgressToNextLevel()
        {
            CurrentLevel.startDoor.SetActive(true);
            
            _levelIndex++;
            if (_levelIndex < Levels.Count)
            {
                Levels[_levelIndex].StartLevel();
                Levels[_levelIndex - 1].gameObject.SetActive(false);
            }
            else
            {
                // TODO: Game completion
                Debug.Log("No next level to start!");

                Time.timeScale = 0;
                Player.playerMovement.DisablePlayerMovement();
                _gameHUD.ShowCompletionBackground();
            }
        }
    }
}
