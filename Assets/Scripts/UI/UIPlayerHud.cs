using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game;
using Input;
using Interactions;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
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
        
        [SerializeField] private TooltipSO _tooltipStorage;
        [SerializeField] private Interactable _interactableStorage;
        
        
        [SerializeField] private Image _crosshair;
        [SerializeField] private Image _enemyLocation;

        private const float LOW_HEALTH_THRESHOLD = 0.4f;
        private const float ROPE_ICON_ROTATION_SPEED = 125f;
        
        private List<InputAction> _bindingsLookup;
        private InputSystem_Actions _inputSystemActions;

        public void Start()
        {
            _inputSystemActions = new InputSystem_Actions();
            _bindingsLookup = new List<InputAction>()
            {
                _inputSystemActions.Player.Move,
                _inputSystemActions.Player.Look,
                _inputSystemActions.Player.Jump,
                _inputSystemActions.Player.Crouch,
                _inputSystemActions.Player.Attack,
                _inputSystemActions.Player.Interact,
                _inputSystemActions.Player.Restart
            };
        }
        
        public void FixedUpdate()
        {
            if (_ropeIcon.enabled)
            {
                _ropeIcon.rectTransform.Rotate(Vector3.forward * (ROPE_ICON_ROTATION_SPEED * Time.deltaTime));
            }
        }

        public void TurnOffTooltip()
        {
            _tooltipBackground.gameObject.SetActive(false);
            _textPopupBackground.gameObject.SetActive(false);
        }

        public void PointToEnemy(float angle, float distance)
        {
            float angleRad = angle * Mathf.Deg2Rad;
            
            float x = _crosshair.rectTransform.anchoredPosition.x + 64f * Mathf.Cos(-angleRad);
            float y = _crosshair.rectTransform.anchoredPosition.y + 64f * Mathf.Sin(-angleRad);

            // Update the element's position
            _enemyLocation.rectTransform.anchoredPosition = new Vector2(x, y);
            _enemyLocation.rectTransform.rotation = Quaternion.Euler(0, 0, -angle);
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

        public void UpdateInteractText(Interactable interactable, bool status)
        {
            if (interactable == null)
            {
                _interactText.gameObject.SetActive(false);
                _tooltipStorage = null;
                return;
            }
            
            _interactableStorage = interactable;
            
            _textPopupBackground.gameObject.SetActive(status);
            if (!_interactText.gameObject.activeSelf) return;
            
            _interactTextPromptSetter.ReplaceMessage($"[BP_0] {_interactableStorage.GetText()}", _inputSystemActions.Player.Interact);
        }
        public void RefreshInteractText()
        {
            if (!_interactText.gameObject.activeSelf) return;
            
            if (_interactableStorage == null) return;
            
            _interactTextPromptSetter.ReplaceMessage($"[BP_0] {_interactableStorage.GetText()}", _inputSystemActions.Player.Interact);
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
            
            tooltip.InitializeList(_bindingsLookup);
            _tooltipBackground.gameObject.SetActive(tooltip.tooltipText != "");
            if (!_tooltipText.gameObject.activeSelf) return;
            
            _setPromptText.ReplaceMessage(tooltip.tooltipText, tooltip);
        }

        public void RefreshTooltipText()
        {
            if (!_tooltipText.gameObject.activeSelf) return;
            
            if (_tooltipStorage == null) return;
            
            _tooltipStorage.InitializeList(_bindingsLookup);
            _setPromptText.ReplaceMessage(_tooltipStorage.tooltipText, _tooltipStorage);
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
}