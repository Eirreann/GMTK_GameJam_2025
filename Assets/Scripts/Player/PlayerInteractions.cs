using System.Collections.Generic;
using System.Linq;
using Game;
using Interactions;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerInteractions : MonoBehaviour
    {
        public List<Interactable> interactablesInRange;
        
        public void Update()
        {
            if (GameManager.Instance.inputHandler._interact)
            {
                
            }
        }
    }
}