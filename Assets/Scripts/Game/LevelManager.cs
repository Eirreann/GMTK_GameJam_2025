using System.Collections.Generic;
using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        public Rope ropePickup;
        public GameObject depositPoint;
        
        public GameObject endDoor;

        public List<Enemy_Base> enemies;

        private const float INTERACT_DISTANCE = 3f;
        public bool allCaptured;

        public void Update()
        {
            //check if all enemies are captured, if so activate the rope pickup
            if (allCaptured)
            {
                ropePickup.enabled = true;
            }
        }

        public bool PickupRope(bool hasRope)
        {
            if (allCaptured && !hasRope)
            {
                var ropeDis = Vector3.Distance(GameManager.Instance.Player.transform.position, ropePickup.transform.position);
                if (ropeDis < INTERACT_DISTANCE)
                {
                    ropePickup.endPoint = GameManager.Instance.Player.transform;
                    return true;
                }
            }
            
            return false;
        }

        public bool DepositRope(bool hasRope)
        {
            if (allCaptured && hasRope)
            {
                var depositDis = Vector3.Distance(GameManager.Instance.Player.transform.position, depositPoint.transform.position);
                if (depositDis < INTERACT_DISTANCE)
                {
                    ropePickup.endPoint = depositPoint.transform;
                    endDoor.SetActive(false);
                    return false;
                }
            }

            return false;
        }
    }
}