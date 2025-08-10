using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game;
using TMPro;
using UI;
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
        
        [SerializeField] private SetPromptText _setPromptText;
        [SerializeField] private SetPromptText _interactTextPromptSetter;
    
        [SerializeField] private Image _ropeIcon;
        
        [SerializeField] private Image _damagePanel;
        [SerializeField] private Image _fadePanel;
        
        [SerializeField] private TextMeshProUGUI _damageText;
        
        [SerializeField] private TooltipSO _tooltipStorage;

        private const float LOW_HEALTH_THRESHOLD = 0.4f;
        private const float ROPE_ICON_ROTATION_SPEED = 125f;

        public void FixedUpdate()
        {
            if (_ropeIcon.enabled)
            {
                _ropeIcon.rectTransform.Rotate(Vector3.forward * (ROPE_ICON_ROTATION_SPEED * Time.deltaTime));
            }
        }
        
        public void UpdateHealthUI(int current, int max)
        {
            _healthBarFill.fillAmount = (float)current / max;

            _healthBarFill.color = (float)current / max < LOW_HEALTH_THRESHOLD ? Color.red : Color.green;
        }
        
        public void UpdateWallJuiceUI(int current, int max)
        {
            _wallJuiceBarFill.fillAmount = (float)current / max;
        }

        public void UpdateInteractText(String text, List<string> bindings)
        {
            
            
            _textPopupBackground.gameObject.SetActive(text != "");
            if (!_interactText.gameObject.activeSelf) return;
            
            _interactTextPromptSetter.ReplaceMessage(text, "", bindings);
        }

        public void UpdateTooltipText(TooltipSO tooltip)
        {
            if (tooltip == null)
            {
                _tooltipBackground.gameObject.SetActive(false);
                _tooltipStorage = null;
                return;
            }
            
            _tooltipStorage = tooltip;
            
            
            _tooltipBackground.gameObject.SetActive(tooltip.tooltipText != "");
            if (!_tooltipText.gameObject.activeSelf) return;
            
            _setPromptText.ReplaceMessage(tooltip.tooltipText, "", tooltip.bindingsList);
        }

        public void RefreshTooltipText()
        {
            if (!_tooltipText.gameObject.activeSelf) return;
            
            if (_tooltipStorage == null) return;
            _setPromptText.ReplaceMessage(_tooltipStorage.tooltipText, "", _tooltipStorage.bindingsList);
        }

        public void RefreshInteractText()
        {
            if (!_interactText.gameObject.activeSelf) return;
            
            _interactTextPromptSetter.ReplaceMessage("[BP_0] Interact", "", new List<string>() { "Interact" });
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