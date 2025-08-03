using Game;
using UnityEngine;
using UnityEngine.Events;

namespace Interactions
{
    public class Interactable : MonoBehaviour
    {
        private const float INTERACT_DISTANCE = 3.75f;
        public UnityAction<bool> interactableAction;

        public bool triggered = false;
        public bool isEnabled = false;

        public void Update()
        {
            if (Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position) < INTERACT_DISTANCE)
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
                GameManager.Instance.Player.HUD.UpdateInteractText("Interact");
            }
        }
        
        public void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                GameManager.Instance.Player.HUD.UpdateInteractText("");
            }
        }
    }
}