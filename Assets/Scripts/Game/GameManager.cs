using System;
using System.Collections.Generic;
using Input;
using Player;
using UI;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private bool _isPersistent;
        
        public List<LevelManager> Levels;
        public LevelManager CurrentLevel => Levels[_levelIndex];
        public PlayerController Player;
        
        [HideInInspector] public InputHandler inputHandler;
        public ControlsChangedHelper controlsHelper;
        
        [SerializeField] private GameHUD _gameHUD;
        [SerializeField] private int _levelIndex = 0;
        
        private void Start()
        {
            Application.targetFrameRate = PlayerPrefs.GetInt("maxFPS", 30);
            QualitySettings.vSyncCount = PlayerPrefs.GetInt("vSync", 0);
            
            controlsHelper = GetComponent<ControlsChangedHelper>();

            controlsHelper.OnControlsChanged += Player.HUD.RefreshInteractText;
            controlsHelper.OnControlsChanged += Player.HUD.RefreshTooltipText;
            
            inputHandler = GetComponent<InputHandler>();

            if (Levels.Count > 0)
            {
                Levels.ForEach(lvl => lvl.gameObject.SetActive(lvl == CurrentLevel));
                
                Levels[_levelIndex].StartLevel();
            }
            
            CurrentLevel.finalTerminal.Init(EndGame);
        }
        
        public void ResetLevel()
        {
            Player.Reset();
            CurrentLevel.Reset();
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
                CurrentLevel.ReturnRope();
                
                Player.TakeDamage(0);
            }
        }
        
        public void EndGame(bool isEnd)
        {
            Debug.Log("Ending game.");
            inputHandler.OnGameOver();
            _gameHUD.ShowCompletionBackground();
        }

        public void ProgressToNextLevel()
        {
            Debug.Log("Progressing to next level.");
            _levelIndex++;

            if (_levelIndex == 2)
            {
                AudioManager.Instance.ProgressMusic();  
            }
            
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
                
                Player.SetRespawnLocation(CurrentLevel.PlayerRespawnLocation);
                Player.Reset();
                
                Levels[_levelIndex].StartLevel();

                AudioManager.Instance.FadeOutMusic();

                CurrentLevel.finalTerminal.isEnabled = true;
                CurrentLevel.finalTerminal.SetReadyForCompletion();
            }
        }
    }
}
