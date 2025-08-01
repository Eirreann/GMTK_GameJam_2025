using System.Collections.Generic;
using GogoGaga.OptimizedRopesAndCables;
using Interactions;
using UnityEngine;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        public Interactable ropePickup;
        public Interactable depositPoint;
        
        public GameObject endDoor;

        public List<Enemy_Base> enemies;

        public bool allCaptured;

        [SerializeField] private MeshRenderer pickupCubeRend;
        [SerializeField] private MeshRenderer depositPointCubeRend;

        public bool playerHasRope = false;

        public void StartLevel()
        {
            // TODO
            _setRopeActive(allCaptured);
            
            ropePickup.Init(PickupRope);
            depositPoint.Init(DepositRope);
        }

        public void RegisterEnemyCaptured()
        {
            allCaptured = enemies.TrueForAll(e => e.IsTrapped);
            _setRopeActive(allCaptured);
        }

        public void PickupRope(bool hasRope)
        {
            ropePickup.GetComponent<RopePickup>().rope.endPoint = GameManager.Instance.Player.transform;
        }

        public void DepositRope(bool hasRope)
        {
            ropePickup.GetComponent<RopePickup>().rope.endPoint = depositPoint.transform;
            endDoor.SetActive(false);
        }

        private void _setRopeActive(bool isActive)
        {
            
            if (isActive)
            {
                ropePickup.gameObject.SetActive(true);
                
                pickupCubeRend.material.color = Color.green;
                depositPointCubeRend.material.color = Color.green;
            }
            else
            {
                ropePickup.gameObject.SetActive(false);
                
                pickupCubeRend.material.color = Color.red;
                depositPointCubeRend.material.color = Color.red;
            }
        }
    }
}