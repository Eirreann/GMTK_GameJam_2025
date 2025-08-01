using System.Collections.Generic;
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

        private int _levelIndex = 0;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void Awake()
        {
            inputHandler = GetComponent<InputHandler>();
            
            Init(_isPersistent);
        }

        public override void Init(bool isPersist = false)
        {
            base.Init(isPersist);

            if (Levels.Count > 0)
                Levels[_levelIndex].StartLevel();
        }

        public void ProgressToNextLevel()
        {
            _levelIndex++;
            if(_levelIndex < Levels.Count)
                Levels[_levelIndex].StartLevel();
            else
            {
                // TODO: Game completion
            }
        }
    }
}
