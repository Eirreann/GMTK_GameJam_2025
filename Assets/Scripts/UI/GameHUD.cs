using Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class GameHUD : MonoBehaviour
    {
        [Header("Pause Menu")]
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private Button _continueBtn;
        [SerializeField] private Button _menuBtn;
        
        [Header("End Game")]
        [SerializeField] private GameObject _completionBackground;
        [SerializeField] private Button _endGameButton;

        private bool _pauseActive = false;
        
        private void Shutdown()
        {
            GameManager.Instance.Player.HUD.Fade(true);
            SceneManager.LoadScene(0);
        }
        
        public void Start()
        {
            _continueBtn.onClick.AddListener(ShowPauseMenu);
            _endGameButton.onClick.AddListener(Shutdown);
            _menuBtn.onClick.AddListener(_returnToMenu);
        }

        public void ShowPauseMenu()
        {
            _pauseActive = !_pauseActive;
            _pauseMenu.SetActive(_pauseActive);
            GameManager.Instance.Player.playerMovement.DisablePlayerMovement(_pauseActive);
            Time.timeScale = _pauseActive ? 0 : 1;
        }
        
        public void ShowCompletionBackground()
        {
            Cursor.lockState = CursorLockMode.None;
            GameManager.Instance.Player.playerMovement.DisablePlayerMovement(true);
            
            _completionBackground.SetActive(true);
        }

        private void _returnToMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
}