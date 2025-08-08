using System;
using Game;
using UnityEngine;

namespace Interactions
{
    public class ProximityInteract : Interactable
    {
        [SerializeField] private TooltipSO tooltipObj;
        private String _text;

        public void Start()
        {
            _text = tooltipObj.tooltipText;
        }
        
        public void Update()
        {
            
        }
        
        public void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                GameManager.Instance.Player.HUD.UpdateTooltipText(_text);
            }
        }
        
        public void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                GameManager.Instance.Player.HUD.UpdateTooltipText("");
            }
        }
    }
}