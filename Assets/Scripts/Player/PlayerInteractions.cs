using Game;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerInteractions : MonoBehaviour
    {
        public bool holding_rope = false;
        
        public void Update()
        {
            if (GameManager.Instance.inputHandler._interact)
            {
                if(!holding_rope) holding_rope = GameManager.Instance.CurrentLevel.PickupRope(holding_rope);
                else holding_rope =  GameManager.Instance.CurrentLevel.DepositRope(holding_rope);
            }
        }
    }
}