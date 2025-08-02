using Game;
using UnityEngine;
using UnityEngine.Events;

namespace Interactions
{
    public class Interactable : MonoBehaviour
    {
        private const float INTERACT_DISTANCE = 3f;
        public UnityAction<bool> interactableAction;

        public bool triggered = false;

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

        public virtual bool Interact(bool status)
        {
            triggered = status;
            
            interactableAction.Invoke(status);
            return status;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && !triggered)
            {
                GameManager.Instance.Player.playerStats.UpdateInteractText("Interact");
            }
        }
        
        public void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                GameManager.Instance.Player.playerStats.UpdateInteractText("");
            }
        }
    }
}