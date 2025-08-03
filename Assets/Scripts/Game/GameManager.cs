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

        [SerializeField] public int _levelIndex = 0;

        private void Start()
        {
            inputHandler = GetComponent<InputHandler>();

            if (Levels.Count > 0)
                Levels[_levelIndex].StartLevel();
        }
        
        public void ResetLevel()
        {
            CurrentLevel.Reset();
            Player.Reset();
        }

        private void Update()
        {
            if(inputHandler._reset)
                ResetLevel();

            if (inputHandler._pause)
                _gameHUD.ShowPauseMenu();
            
            if (Player.transform.position.y < -5f || Player.transform.position.y > 20f)
            {
                ResetLevel();
                GameManager.Instance.CurrentLevel.ReturnRope();
            }
        }
        
        public void EndGame(bool isEnd)
        {
            Debug.Log("Ending game.");
            _gameHUD.ShowCompletionBackground();
        }

        public void ProgressToNextLevel()
        {
            _levelIndex++;
            if (_levelIndex < Levels.Count)
            {
                CurrentLevel.startDoor.SetActive(true);
                Levels[_levelIndex].StartLevel();
                Levels[_levelIndex - 1].gameObject.SetActive(false);

                Player.transform.position = Levels[_levelIndex].PlayerRespawnLocation.position;
                
                Player.playerStats.IncreaseMaxJuice(5);
                
                Player.playerStats.ReplenishAllHealth();
                Player.playerStats.ReplenishAllJuice();
            }
            else
            {
                _levelIndex = 0;
                Player.playerMovement.SetRespawn(CurrentLevel.PlayerRespawnLocation);
                Player.playerMovement.ResetPlayer();
                Levels[_levelIndex].StartLevel();


                CurrentLevel.finalTerminal.isEnabled = true;
                CurrentLevel.finalTerminal.SetReadyForCompletion();
                CurrentLevel.finalTerminal.Init(EndGame);
            }
        }
    }
}
