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
            if (GameManager.Instance.inputHandler._interact && Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position) < INTERACT_DISTANCE)
            {
                Interact(!triggered);
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
    }
}