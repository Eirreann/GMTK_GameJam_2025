using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHud : MonoBehaviour
{
        [SerializeField] private Image _healthBarFill;
        [SerializeField] private Image _wallJuiceBarFill;

        [SerializeField] private Image _textPopupBackground;
        [SerializeField] private TextMeshProUGUI _interactText;

        [SerializeField] private Image _tooltipBackground;
        [SerializeField] private TextMeshProUGUI _tooltipText;

        public void UpdateHealthUI(int current, int max)
        {
            _healthBarFill.fillAmount = (float)current / max;
        }
        
        public void UpdateWallJuiceUI(int current, int max)
        {
            _wallJuiceBarFill.fillAmount = (float)current / max;
        }

        public void UpdateInteractText(String text)
        {
            _textPopupBackground.gameObject.SetActive(text != "");
            _interactText.text = text;
        }

        public void UpdateTooltipText(String text)
        {
            _tooltipBackground.gameObject.SetActive(text != "");
            _tooltipText.text = text;
        }
}