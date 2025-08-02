using System.Collections.Generic;
using GogoGaga.OptimizedRopesAndCables;
using Interactions;
using Unity.VisualScripting;
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

        public void ReturnRope()
        {
            Debug.Log("Returning rope");
            
            playerHasRope = false;
            ropePickup.GetComponent<RopePickup>().rope.endPoint = ropePickup.transform;
        }

        public void PickupRope(bool hasRope)
        {
            playerHasRope = hasRope;
            ropePickup.GetComponent<RopePickup>().rope.endPoint = GameManager.Instance.Player.transform;
        }

        public void DepositRope(bool hasRope)
        {
            ropePickup.GetComponent<RopePickup>().rope.endPoint = depositPoint.transform;
            if (hasRope)
            {
                playerHasRope = false;
                endDoor.SetActive(false);
                DestroyEnemies();
            }
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

        private void DestroyEnemies()
        {
            foreach (var e in enemies)
            {
                Destroy(e.gameObject);
            }
            enemies.Clear();
        }
    }
}