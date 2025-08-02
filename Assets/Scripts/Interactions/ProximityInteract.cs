using Game;
using UnityEngine;

namespace Interactions
{
    public class ProximityInteract : Interactable
    {
        private const float INTERACT_DISTANCE = 3f;
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
                GameManager.Instance.Player.playerStats.UpdateTooltipText("Tooltip Text Here");
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