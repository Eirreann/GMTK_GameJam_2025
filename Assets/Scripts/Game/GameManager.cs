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
        
        public void ResetLevel()
        {
            CurrentLevel.Reset();
            Player.Reset();
        }

        private void Update()
        {
            if(inputHandler._reset)
                ResetLevel();
            
            if (Player.transform.position.y < -5f || Player.transform.position.y > 20f)
            {
                ResetLevel();
                GameManager.Instance.CurrentLevel.ReturnRope();
            }
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
                // TODO: Game completion
                Debug.Log("No next level to start!");

                Time.timeScale = 0;
                Player.playerMovement.DisablePlayerMovement();
                _gameHUD.ShowCompletionBackground();
            }
        }
    }
}
