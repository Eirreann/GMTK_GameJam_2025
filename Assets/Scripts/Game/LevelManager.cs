using System.Collections.Generic;
using GogoGaga.OptimizedRopesAndCables;
using Interactions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        public List<WallParent> LevelWalls = new List<WallParent>();
        
        public Transform PlayerRespawnLocation;
        
        public Interactable ropePickup;
        public Interactable ropeDeposit;

        public Transform ropePickupLocation;
        public Transform ropeDepositLocation;

        public EndGoalInteractable finalTerminal;

        public GameObject startDoor;
        public GameObject endDoor;
        
        public Animator endDoorAnimator;

        public List<Enemy_Base> enemies;

        public bool allCaptured;

        [SerializeField] private MeshRenderer pickupCubeRend;
        [SerializeField] private MeshRenderer depositPointCubeRend;

        [SerializeField] private Material _disabledMat;
        [SerializeField] private Material _enabledMat;

        public bool playerHasRope = false;

        public UnityEvent onLevelStart;
        public BoxCollider endDoorCollider;

        public void StartLevel()
        {
            gameObject.SetActive(true);
            if(onLevelStart != null) onLevelStart.Invoke();
            
            // TODO
            _setRopeActive(allCaptured);
            
            ropePickup.Init(PickupRope);
            ropeDeposit.Init(DepositRope);
            
            GameManager.Instance.Player.playerMovement.SetRespawn(PlayerRespawnLocation);
        }

        public void Reset()
        {
            if (playerHasRope) ReturnRope();
            if (allCaptured) _setRopeActive(false);
            
            enemies.ForEach(e => e.ResetEnemy());
            
            DestroyLevelWalls();
        }

        public void RegisterEnemyCaptured()
        {
            allCaptured = enemies.TrueForAll(e => e.IsTrapped);
            _setRopeActive(allCaptured);
        }
        
        public void PickupRope(bool hasRope)
        {
            playerHasRope = hasRope;
            
            ropePickup.GetComponent<RopePickup>().rope.endPoint = GameManager.Instance.Player.transform;
            GameManager.Instance.Player.HUD.SetRopeVisible(hasRope);
            ropeDeposit.triggered = false;

            _setRopeActive(hasRope);
            
            AudioManager.Instance.OnPickupRope();
        }

        public void DestroyLevelWalls()
        {
            LevelWalls.ForEach(w => Destroy(w.gameObject));
            LevelWalls.Clear();
        }
        
        public void DepositRope(bool hasRope)
        {
            if (playerHasRope)
            {
                ropePickup.GetComponent<RopePickup>().rope.endPoint = ropeDepositLocation;
                
                GameManager.Instance.Player.HUD.SetRopeVisible(false);
                playerHasRope = false;
                
                endDoorCollider.enabled = false;
                
                DestroyLevelWalls();
                
                // TODO: Check if this deposit point is the final one, as opposed to a tether?
                _endLevel();
                
                AudioManager.Instance.OnDepositRope();
            }
        }

        public void ReturnRope()
        {
            Debug.Log("Returning rope");
            
            GameManager.Instance.Player.HUD.SetRopeVisible(false);
            playerHasRope = false;
            
            ropePickup.GetComponent<RopePickup>().rope.endPoint = ropePickupLocation;
            ropePickup.GetComponent<RopePickup>().triggered = false;
            AudioManager.Instance.OnDropRope();
        }


        private void _setRopeActive(bool isActive)
        {
            ropePickup.isEnabled = isActive;
            pickupCubeRend.material = isActive ? _enabledMat : _disabledMat;

            if (playerHasRope)
            {
                ropeDeposit.isEnabled = isActive;
                depositPointCubeRend.material = isActive ? _enabledMat : _disabledMat;
            }
        }

        private void _endLevel()
        {
            _destroyEnemies();
            endDoorAnimator.Play("Door_Open");
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