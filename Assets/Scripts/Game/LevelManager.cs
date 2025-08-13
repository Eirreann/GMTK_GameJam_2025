using System.Collections.Generic;
using Enemies;
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
            _setRopeActive(allCaptured, playerHasRope);
            
            ropePickup.Init(PickupRope);
            ropeDeposit.Init(DepositRope);
            
            GameManager.Instance.Player.SetRespawnLocation(PlayerRespawnLocation);
        }

        public void Reset()
        {
            if (playerHasRope) ReturnRope();
            
            _setRopeActive(false, false);
            _setRopeMaterials(ropePickup.triggered, ropeDeposit.triggered);
            
            enemies.ForEach(e => e.ResetEnemy());
            
            DestroyLevelWalls();
        }

        public void RegisterEnemyCaptured()
        {
            allCaptured = enemies.TrueForAll(e => e.IsTrapped);
            
            _setRopeActive(allCaptured, playerHasRope);
            _setRopeMaterials(allCaptured, false);
        }
        
        public void PickupRope(bool hasRope)
        {
            playerHasRope = hasRope;
            
            ropePickup.GetComponent<RopePickup>().rope.endPoint = GameManager.Instance.Player.transform;
            GameManager.Instance.Player.HUD.SetRopeVisible(hasRope);
            
            ropeDeposit.triggered = false;

            _setRopeActive(allCaptured, playerHasRope);
            _setRopeMaterials(hasRope, hasRope);
            
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
            ropePickup.triggered = false;
            AudioManager.Instance.OnDropRope();
        }


        private void _setRopeActive(bool pickupActive, bool depositActive)
        {
            ropePickup.isEnabled = pickupActive;
            ropeDeposit.isEnabled = depositActive;
        }

        private void _setRopeMaterials(bool pickupActive, bool depositActive)
        {
            pickupCubeRend.material = pickupActive ? _enabledMat : _disabledMat;
            depositPointCubeRend.material = depositActive ? _enabledMat : _disabledMat;
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