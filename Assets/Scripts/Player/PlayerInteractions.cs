using System.Collections.Generic;
using System.Linq;
using Game;
using Input;
using Interactions;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerInteractions : MonoBehaviour
    {
        public Camera playerCamera;
        public bool playerCanInteract = false;
        
        private Interactable _interactableLookedAt;
        public LayerMask interactableLayer;
        
        public void Update()
        {
            if (playerCanInteract)
            {
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 3f, interactableLayer, QueryTriggerInteraction.Ignore))
                {
                    if (_interactableLookedAt == null)
                    {
                        _interactableLookedAt = hit.collider.GetComponent<Interactable>();
                        _interactableLookedAt.EnableInteractableText();
                    }
                    
                    if (InputHandler.Instance._interact && _interactableLookedAt != null)
                    {
                        _interactableLookedAt.Interact(true);
                    }
                }
                else
                {
                    if (_interactableLookedAt != null)
                    {
                        _interactableLookedAt.DisableInteractableText();
                        _interactableLookedAt = null;
                    }
                }
            }
        }
    }
}