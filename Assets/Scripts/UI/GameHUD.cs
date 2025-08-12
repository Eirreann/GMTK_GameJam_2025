using Game;
using UnityEngine;
using UnityEngine.EventSystems;
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
        
        [Header("Panels")]
        [SerializeField] private GameObject _menuPanel;
        [SerializeField] private GameObject _controlsPanel;
        [SerializeField] private GameObject _settingsPanel;
        
        [Header("Buttons")]
        [SerializeField] private Button _openControlsButton;
        [SerializeField] private Button _openSettingsButton;
        
        [SerializeField] private Button _closeControlsButton;
        [SerializeField] private Button _closeSettingsButton;
        
        
        

        private bool _pauseActive = false;
        public UISelectionHelper uiSelectionHelper;
        
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

            _openSettingsButton.onClick.AddListener(ShowSettings);
            _openControlsButton.onClick.AddListener(ShowControls);

            uiSelectionHelper.OnCloseRoot = ClosePauseMenu;
        }

        public void ClosePauseMenu()
        {
            ShowPauseMenu();
        }
        
        public void ShowPauseMenu()
        {
            _pauseActive = !_pauseActive;
            _pauseMenu.SetActive(_pauseActive);
            GameManager.Instance.Player.DisablePlayerMovement(_pauseActive);
            Time.timeScale = _pauseActive ? 0 : 1;
            
            if (_pauseActive)
            {
                uiSelectionHelper.AddActivePanel(_menuPanel);
                uiSelectionHelper.GrabLastVisitedButton();
                
                uiSelectionHelper._inputSystem.UI.Enable();
            }
            else
            {
                uiSelectionHelper.RemoveActivePanel(_menuPanel);
                uiSelectionHelper._inputSystem.UI.Disable();
            }
        }

        public void HidePauseMenu()
        {
            _menuPanel.SetActive(false);
        }
        
        public void ShowCompletionBackground()
        {
            Cursor.lockState = CursorLockMode.None;
            GameManager.Instance.Player.DisablePlayerMovement(true);
            
            uiSelectionHelper.SetLastSelected(_endGameButton.gameObject);
            uiSelectionHelper.SetLastVisited(_endGameButton.gameObject);
            
            _completionBackground.SetActive(true);
        }

        private void ShowControls()
        {
            _controlsPanel.SetActive(true);
            HidePauseMenu();
                
            uiSelectionHelper.AddActivePanel(_controlsPanel);
            uiSelectionHelper.SetLastSelected(_openControlsButton.gameObject);
            
            EventSystem.current.SetSelectedGameObject(_closeControlsButton.gameObject);
        }

        private void ShowSettings()
        {
            _settingsPanel.SetActive(true);
            HidePauseMenu();
            
            uiSelectionHelper.AddActivePanel(_settingsPanel);
            uiSelectionHelper.SetLastSelected(_openSettingsButton.gameObject);
            
            EventSystem.current.SetSelectedGameObject(_closeSettingsButton.gameObject);
        }

        private void _returnToMenu()
        {
            Time.timeScale = 1;
            GameManager.Instance.inputHandler.OnGameOver();
            SceneManager.LoadScene(0);
        }
    }
}