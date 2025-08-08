using Game;
using UnityEngine;
using UnityEngine.Events;

namespace Interactions
{
    public class Interactable : MonoBehaviour
    {
        private bool _playerInRange = false;
        public UnityAction<bool> interactableAction;

        private string _buttonNumber = "1";
        [SerializeField] private string _interactText = "Interact";

        public bool triggered = false;
        public bool isEnabled = false;

        public void Update()
        {
            if (isEnabled && _playerInRange)
            {
                if (GameManager.Instance.inputHandler._interact)
                {
                    if(!triggered) Interact(true);
                }
            }
        }

        public virtual void Init(UnityAction<bool> uAction)
        {
            interactableAction = uAction;
        }

        public virtual void ToggleEnabled()
        {
            enabled = !enabled;
            isEnabled = enabled;
        }

        public virtual bool Interact(bool status)
        {
            if (isEnabled)
            {
                triggered = status;
                GameManager.Instance.Player.HUD.UpdateInteractText("");
            
                interactableAction.Invoke(status);
                return status;
            }
            return status;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && isEnabled && !triggered)
            {
                _playerInRange = true;
                GameManager.Instance.Player.HUD.UpdateInteractText($"<sprite={_buttonNumber}> {_interactText}");
            }
        }
        
        public void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                _playerInRange = false;
                GameManager.Instance.Player.HUD.UpdateInteractText("");
            }
        }
    }
}