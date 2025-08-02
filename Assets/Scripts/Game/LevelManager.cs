using System.Collections.Generic;
using GogoGaga.OptimizedRopesAndCables;
using Interactions;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        [HideInInspector] public List<WallParent> LevelWalls = new List<WallParent>();
        
        public Transform PlayerRespawnLocation;
        public Interactable ropePickup;
        public Interactable depositPoint;

        public GameObject startDoor;
        public GameObject endDoor;

        public List<Enemy_Base> enemies;

        public bool allCaptured;

        [SerializeField] private MeshRenderer pickupCubeRend;
        [SerializeField] private MeshRenderer depositPointCubeRend;

        public bool playerHasRope = false;

        public void StartLevel()
        {
            gameObject.SetActive(true);
            
            // TODO
            _setRopeActive(allCaptured);
            depositPoint.gameObject.SetActive(false);
            
            ropePickup.Init(PickupRope);
            depositPoint.Init(DepositRope);
            
            GameManager.Instance.Player.playerMovement.SetRespawn(PlayerRespawnLocation);
        }

        public void Reset()
        {
            if(playerHasRope)
                ReturnRope();
            enemies.ForEach(e => e.ResetEnemy());
            
            LevelWalls.ForEach(w => w.DestroyWall());
            LevelWalls.Clear();
        }

        public void RegisterEnemyCaptured()
        {
            allCaptured = enemies.TrueForAll(e => e.IsTrapped);
            _setRopeActive(allCaptured);
        }

        public void ReturnRope()
        {
            Debug.Log("Returning rope");
            
            GameManager.Instance.Player.playerStats.SetRopeVisible(false);
            playerHasRope = false;
            
            ropePickup.GetComponent<RopePickup>().rope.endPoint = ropePickup.transform;
            ropePickup.GetComponent<RopePickup>().triggered = false;
        }

        public void PickupRope(bool hasRope)
        {
            playerHasRope = hasRope;
            ropePickup.GetComponent<RopePickup>().rope.endPoint = GameManager.Instance.Player.transform;
            GameManager.Instance.Player.playerStats.SetRopeVisible(hasRope);
            
            depositPoint.gameObject.SetActive(true);
        }

        public void DepositRope(bool hasRope)
        {
            if (playerHasRope)
            {
                ropePickup.GetComponent<RopePickup>().rope.endPoint = depositPoint.transform;
                
                GameManager.Instance.Player.playerStats.SetRopeVisible(false);
                playerHasRope = false;
                
                // TODO: Check if this deposit point is the final one, as opposed to a tether?
                _endLevel();
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

        private void _endLevel()
        {
            _destroyEnemies();
            
            endDoor.SetActive(false);
            
            if (GameManager.Instance._levelIndex < GameManager.Instance.Levels.Count)
            {
                GameManager.Instance.Levels[GameManager.Instance._levelIndex + 1].gameObject.SetActive(true);
            }
            
        }

        private void _destroyEnemies()
        {
            foreach (var e in enemies)
            {
                Destroy(e.gameObject);
            }
            enemies.Clear();
        }
    }
}