using Game;
using UnityEngine;
using Utilities;

namespace Player.PlayerStates
{
    public class CrouchingState : IState
    {
        private PlayerController player;

        public CrouchingState(PlayerController player)
        {
            this.player = player;
        }

        public void Enter()
        {
            player.transform.localScale = new Vector3(player.transform.localScale.x, 1f, player.transform.localScale.z);
        }

        public void Update()
        {
            player.playerMovement.ProcessMovement(player.playerMovement.playerSpeed * 0.5f);
            
            if (!GameManager.Instance.inputHandler._crouch)
            {
                if (!Physics.Raycast(player.playerCamera.transform.position, Vector3.up, out RaycastHit hit, 1.25f))
                {
                    player._playerStateMachine.ChangeState(player._playerStateMachine.idleState);
                }
            }
        }

        public void FixedUpdate()
        {
        }

        public void Exit()
        {
            player.transform.localScale = new Vector3(player.transform.localScale.x, 2f, player.transform.localScale.z);
        }
    }
}