using Game;
using TMPro;
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
        [SerializeField] private GameObject _settingsPanel;
        
        [Header("Buttons")]
        [SerializeField] private Button _openSettingsButton;
        
        [SerializeField] private Button _closeSettingsButton;
        
        private bool _pauseActive = false;
        public UISelectionHelper uiSelectionHelper;

        [SerializeField] private float _timer = 59f;
        [SerializeField] private TextMeshProUGUI _timerDisplay;
        
        private void Shutdown()
        {
            Time.timeScale = 1;
            GameManager.Instance.Player.HUD.Fade(true);
            SceneManager.LoadScene(0);
            
            uiSelectionHelper._inputSystem.UI.Disable();
        }
        
        public void Start()
        {
            uiSelectionHelper = GetComponent<UISelectionHelper>();
            
            _continueBtn.onClick.AddListener(ShowPauseMenu);
            _endGameButton.onClick.AddListener(Shutdown);
            _menuBtn.onClick.AddListener(_returnToMenu);

            _openSettingsButton.onClick.AddListener(ShowSettings);

            uiSelectionHelper.OnCloseRoot = ClosePauseMenu;
        }

        private void Update()
        {
            if (_timerDisplay.gameObject.activeSelf)
            {
                _timer += Time.deltaTime % 60;
                
                var minutes = (int)_timer / 60;
                var seconds = (int)_timer % 60;
                int milliseconds = (int)(_timer * 1000 - (1000 * seconds));
                
                _timerDisplay.text = string.Format("{00:00}:{01:00}.{02:00}", minutes, (int)_timer % 60 - (minutes / 60), milliseconds.ToString("D3"));
            }
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
                uiSelectionHelper._inputSystem.Player.Disable();
                uiSelectionHelper.GrabLastSelectedButton();
                uiSelectionHelper.AddActivePanel(_menuPanel);
                uiSelectionHelper._inputSystem.UI.Enable();
            }
            else
            {
                uiSelectionHelper.SetLastSelected(_continueBtn.gameObject);
                uiSelectionHelper.RemoveActivePanel(_menuPanel);
                uiSelectionHelper._inputSystem.UI.Disable();
                uiSelectionHelper._inputSystem.Player.Enable();
            }
        }

        public void HidePauseMenu()
        {
            _menuPanel.SetActive(false);
        }
        
        public void ShowCompletionBackground()
        {
            _completionBackground.SetActive(true);
            uiSelectionHelper.SetLastSelected(_endGameButton.gameObject);
            
            GameManager.Instance.Player.DisablePlayerMovement(_pauseActive);
            Cursor.lockState = CursorLockMode.None;
            
            uiSelectionHelper.GrabLastSelectedButton();
            uiSelectionHelper._inputSystem.UI.Enable();
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
            
            uiSelectionHelper._inputSystem.UI.Disable();
        }
    }
}