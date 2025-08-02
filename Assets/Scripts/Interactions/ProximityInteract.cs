using System;
using Game;
using UnityEngine;

namespace Interactions
{
    public class ProximityInteract : Interactable
    {
        [SerializeField] private TooltipSO tooltipObj;
        private String _text;
        private const float INTERACT_DISTANCE = 3f;

        public void Start()
        {
            _text = tooltipObj.tooltipText;
        }
        
        public void Update()
        {
            if (Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position) < INTERACT_DISTANCE)
            {
                
            }
        }
        
        public void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                GameManager.Instance.Player.playerStats.UpdateTooltipText(_text);
            }
        }
        
        public void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                GameManager.Instance.Player.playerStats.UpdateTooltipText("");
            }
        }
    }
}