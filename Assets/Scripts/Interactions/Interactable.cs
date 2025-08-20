using System.Collections.Generic;
using Game;
using Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Interactions
{
    public class Interactable : MonoBehaviour
    {
        private PlayerController _playerController;
        private bool _playerInRange = false;
        public UnityAction<bool> interactableAction;

        private string _buttonNumber = "1";
        [SerializeField] private string _interactText = "Interact";

        public bool triggered = false;
        public bool isEnabled = false;

        public void Start()
        {
            _playerController = FindFirstObjectByType<PlayerController>();
        }
        
        public void Update()
        {
            
        }

         public virtual string GetText()
        {
            return _interactText;
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
                GameManager.Instance.Player.HUD.UpdateInteractText(this, false);
            
                interactableAction.Invoke(status);
            }
            
            return status;
        }

        public void EnableInteractableText()
        {
            if (isEnabled && !triggered)
            {
                GameManager.Instance.Player.HUD.UpdateInteractText(this, true);
            }
        }

        public void DisableInteractableText()
        {
            GameManager.Instance.Player.HUD.UpdateInteractText(this, false);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                other.gameObject.GetComponent<PlayerController>().playerCanInteract = true;
            }
        }
        
        public void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                other.gameObject.GetComponent<PlayerController>().playerCanInteract = false;
            }
        }
    }
}