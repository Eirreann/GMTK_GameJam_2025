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
            Application.Quit();
        }
        
        public void Start()
        {
            _endGameButton.onClick.AddListener(Shutdown);
        }
        
        public void ShowCompletionBackground()
        {
            _completionBackground.SetActive(true);
        }
    }
}