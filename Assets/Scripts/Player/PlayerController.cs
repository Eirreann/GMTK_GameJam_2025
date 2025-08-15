using System;
using Game;
using Interactions;
using Player;
using Player.PlayerStates;
using UI;
using UnityEngine;
using Utilities;

namespace Player
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        public UIPlayerHud HUD;
        
        [HideInInspector] public PlayerMovement playerMovement;
        [HideInInspector] public PlayerStats playerStats;
        [HideInInspector] public PlayerInteractions playerInteractions;
        [HideInInspector] public Player_WallHandler wallHandler;

        public PlayerStateMachine _playerStateMachine;

        public Camera playerCamera;
        public Rigidbody rb;

        public String stateString = "";
        
        public bool playerCanMove = true;
        private Transform _respawnLocation = null;

        public bool canWallJump = false;
        public Transform lastWallJumped;

        public bool hasWallJumped = false;
        
        public void Awake() 
        {
            rb = GetComponent<Rigidbody>();
            
            playerMovement = GetComponent<PlayerMovement>();
            playerStats = GetComponent<PlayerStats>();
            playerInteractions = GetComponent<PlayerInteractions>();
            wallHandler = GetComponent<Player_WallHandler>();

            _playerStateMachine = new PlayerStateMachine(this);
            if (_playerStateMachine != null)
            {
                _playerStateMachine.Initialize(_playerStateMachine.idleState);
            }
        }

        public Vector3 XZHelper(Vector3 vec)
        {
            return new Vector3(vec.x, 0 ,vec.z);
        }

        private void Update()
        {
            stateString = _playerStateMachine.CurrentState.ToString();
            if (playerCanMove)
            {
                playerMovement.RotatePlayer(GameManager.Instance.inputHandler._lookDirection);
                _playerStateMachine.Update();
            }

            // foreach (var enemy in GameManager.Instance.CurrentLevel.enemies)
            // {
            //     // Calculate the direction vector from the player to the object
            //     Vector3 direction = XZHelper(enemy.transform.position) - XZHelper(playerCamera.transform.position);
            //
            //     // Normalize the vectors
            //     Vector3 playerForward = XZHelper(playerCamera.transform.forward.normalized);
            //     Vector3 objectDirection = direction.normalized;
            //
            //     // Calculate the angle
            //     float angle = Vector3.Angle(playerForward, objectDirection);
            //     float signedAngle = Vector3.SignedAngle(playerForward, direction, Vector3.up);
            //     
            //     float distance = Vector3.Distance(transform.position, enemy.transform.position);
            //
            //     Debug.Log($"Angle: {signedAngle}, Distance: {distance}");
            //     
            //     HUD.PointToEnemy(signedAngle, distance);
            // }
        }

        private void FixedUpdate()
        {
            if (playerCanMove)
            {
                _playerStateMachine.FixedUpdate();
            }
        }

        private void Start()
        {
            HUD.Fade(false);
        }

        public void Reset()
        {
            wallHandler.ResetWalls();
            
            gameObject.SetActive(false);
            
            transform.position = _respawnLocation.position;
            playerMovement.xRotation = 0f;
            playerMovement.yRotation = 0f;
            
            gameObject.SetActive(true);
            
            playerStats.ReplenishAllHealth();
            playerStats.ReplenishAllJuice();
            HUD.Fade(false);
        }
        
        public void DisablePlayerMovement(bool state)
        {
            playerCanMove = !state;
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void TakeDamage(int damage)
        {
            var health = playerStats.DamagePlayer(damage);
            HUD.DamageFlash();
            
            AudioManager.Instance.OnTakeDamage();
            
            if (health <= 0)
            {
                GameManager.Instance.ResetLevel();
            }
        }

        public void SetRespawnLocation(Transform respawnLocation)
        {
            _respawnLocation = respawnLocation;
        }
    }

}