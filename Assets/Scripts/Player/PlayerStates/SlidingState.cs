using Game;
using UnityEngine;
using Utilities;

namespace Player.PlayerStates
{
    public class SlidingState : IState
    {
        private PlayerController player;

        public SlidingState(PlayerController player)
        {
            this.player = player;
        }

        private bool _hasSlid;
        public void Enter()
        {
            player.transform.localScale = new Vector3(player.transform.localScale.x, 1f, player.transform.localScale.z);
            _hasSlid = true;
        }

        public void Update()
        {
            if (GameManager.Instance.inputHandler._crouch)
            {
                if (player.rb.linearVelocity.magnitude < 2f)
                {
                    player._playerStateMachine.ChangeState(player._playerStateMachine.crouchingState);
                }
            }
            else
            {
                player._playerStateMachine.ChangeState(player._playerStateMachine.idleState);
            }
            
            if (GameManager.Instance.inputHandler._jump)
            {
                player._playerStateMachine.ChangeState(player._playerStateMachine.jumpingState);
            }
        }

        public void FixedUpdate()
        {
            if (_hasSlid)
            {
                player.rb.AddForce(new Vector3(player.rb.linearVelocity.x * player.playerMovement.slideForce, 0, player.rb.linearVelocity.z * player.playerMovement.slideForce), ForceMode.Impulse);
                _hasSlid = false;
            }
            
            player.rb.AddForce(-player.rb.linearVelocity * 7.5f, ForceMode.Force);
        }
        
        public void Exit()
        {
            player.transform.localScale = new Vector3(player.transform.localScale.x, 2f, player.transform.localScale.z);
        }
    }
}