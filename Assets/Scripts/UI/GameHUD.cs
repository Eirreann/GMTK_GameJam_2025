using Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] private GameObject _completionBackground;
        [SerializeField] private Button _endGameButton;

        private void Shutdown()
        {
            GameManager.Instance.Player.HUD.Fade(true);
        }
        
        public void Start()
        {
            _endGameButton.onClick.AddListener(Shutdown);
        }
        
        public void ShowCompletionBackground()
        {
            Cursor.lockState = CursorLockMode.None;
            GameManager.Instance.Player.playerMovement.DisablePlayerMovement();
            
            _completionBackground.SetActive(true);
        }
    }
}