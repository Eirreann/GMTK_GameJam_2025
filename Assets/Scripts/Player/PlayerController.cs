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
        
        public bool playerCanMove = true;
        private Transform _respawnLocation;

        public Transform lastWallJumped;
        public bool hasWallJumped;
        
        public void Awake() 
        {
            rb = GetComponent<Rigidbody>();
            
            playerMovement = GetComponent<PlayerMovement>();
            playerStats = GetComponent<PlayerStats>();
            playerInteractions = GetComponent<PlayerInteractions>();
            wallHandler = GetComponent<Player_WallHandler>();

            HUD = FindFirstObjectByType<UIPlayerHud>();

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
            if (playerCanMove)
            {
                playerMovement.RotatePlayer(GameManager.Instance.inputHandler._lookDirection);
                _playerStateMachine.Update();
            }
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
            HUD.TurnOffTooltip();
            
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