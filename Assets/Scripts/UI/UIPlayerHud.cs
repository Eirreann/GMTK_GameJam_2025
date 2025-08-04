using System;
using System.Collections;
using DG.Tweening;
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
    
        [SerializeField] private Image _ropeIcon;
        
        [SerializeField] private Image _damagePanel;
        [SerializeField] private Image _fadePanel;
        
        public void UpdateHealthUI(int current, int max)
        {
            _healthBarFill.fillAmount = (float)current / max;

            _healthBarFill.color = (float)current / max < 0.4 ? Color.red : Color.green;
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

        public void SetRopeVisible(bool visible)
        {
            _ropeIcon.enabled = visible;
        }

        public void Fade(bool state)
        {
            var color = _fadePanel.color;
            _fadePanel.color = new Color(color.r, color.g, color.b, state ? 0 : 1);
            _fadePanel.raycastTarget = state;
            _fadePanel.DOFade(state ? 1 : 0, 1f);
        }

        public void DamageFlash()
        {
            StartCoroutine(_takeDamage());
        }

        private IEnumerator _takeDamage()
        {
            _damagePanel.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            _damagePanel.gameObject.SetActive(false);
        }
}