using Game;
using Input;
using UnityEngine;
using Utilities;

namespace Player.PlayerStates
{
    public class IdleState : IState
    {
        private PlayerController player;

        public IdleState(PlayerController player)
        {
            this.player = player;
        }

        public void Enter()
        {
            player.rb.linearVelocity = Vector3.zero;
            player.playerMovement.IsGrounded = true;
            
            player.lastWallJumped = null;
        }

        public void Update()
        {
            player.playerMovement.ProcessMovement(player.playerMovement.playerSpeed);
            if (player.rb.linearVelocity.y < -1f)
            {
                player._playerStateMachine.ChangeState(player._playerStateMachine.fallingState);
            }

            if (InputHandler.Instance._moveDirection.magnitude > 0)
            {
                player._playerStateMachine.ChangeState(player._playerStateMachine.runningState);
            }
            
            if (InputHandler.Instance._jump)
            {
                player._playerStateMachine.ChangeState(player._playerStateMachine.jumpingState);
            }

            if (InputHandler.Instance._crouch)
            {
                player._playerStateMachine.ChangeState(player._playerStateMachine.crouchingState);
            }
        }

        public void FixedUpdate()
        {
        }
        
        public void Exit()
        {
            
        }
    }
}