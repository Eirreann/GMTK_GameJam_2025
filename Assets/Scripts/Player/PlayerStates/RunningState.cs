using Game;
using Input;
using UnityEngine;
using Utilities;

namespace Player.PlayerStates
{
    public class RunningState : IState
    {
        private PlayerController player;

        public RunningState(PlayerController player)
        {
            this.player = player;
        }

        public void Enter()
        {
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
            
            if (InputHandler.Instance._jump)
            {
                player._playerStateMachine.ChangeState(player._playerStateMachine.jumpingState);
            }
            
            if (InputHandler.Instance._crouch)
            {
                player._playerStateMachine.ChangeState(player._playerStateMachine.slidingState);
            }

            if (player.rb.linearVelocity.magnitude <= 0.1f && InputHandler.Instance._moveDirection.magnitude <= 0f)
            {
                player._playerStateMachine.ChangeState(player._playerStateMachine.idleState);
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