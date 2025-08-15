using Game;
using UnityEngine;
using Utilities;

namespace Player.PlayerStates
{
    public class JumpingState : IState
    {
        private PlayerController player;

        public JumpingState(PlayerController player)
        {
            this.player = player;
        }

        private bool _hasJumped;
        private bool _hasWallJumped;
        
        private const float WALL_JUMP_RANGE = 3f;
        
        public void Enter()
        {
            player.playerMovement.IsGrounded = false;
            _hasJumped = true;
            _hasWallJumped = false;
        }

        public void Update()
        {
            player.playerMovement.ProcessMovement(player.playerMovement.playerSpeed);
            
            if (player.rb.linearVelocity.y < -1f)
            {
                player._playerStateMachine.ChangeState(player._playerStateMachine.fallingState);
            }
            
            if (GameManager.Instance.inputHandler._crouch)
            {
                player.transform.localScale = new Vector3(player.transform.localScale.x, 1f, player.transform.localScale.z);
            }
            else
            {
                player.transform.localScale = new Vector3(player.transform.localScale.x, 2f, player.transform.localScale.z);
            }
            
            RaycastHit wallJumpHit;
            Ray leftRay = new Ray(player.playerCamera.transform.position, -player.playerCamera.transform.right);
            Ray rightRay = new Ray(player.playerCamera.transform.position, player.playerCamera.transform.right);

            if (Physics.Raycast(leftRay, out wallJumpHit, WALL_JUMP_RANGE) || Physics.Raycast(rightRay, out wallJumpHit, WALL_JUMP_RANGE))
            {
                player.canWallJump = true;
                if (GameManager.Instance.inputHandler._jump)
                {
                    if (!wallJumpHit.collider.CompareTag("Enemy") && wallJumpHit.collider.transform != player.lastWallJumped)
                    {
                        _hasWallJumped = true;
                        player.lastWallJumped = wallJumpHit.collider.transform;
                    }
                }
            }
            else
            {
                player.canWallJump = false;
            }
        }

        public void FixedUpdate()
        {
            if (_hasJumped)
            {
                player.rb.AddForce(Vector3.up * player.playerMovement.jumpForce, ForceMode.Impulse);
                _hasJumped = false;
            }

            if (_hasWallJumped)
            {
                player.playerMovement.DoWallJump();
                
                _hasWallJumped = false;
            }
        }
        
        public void Exit()
        {
            player.canWallJump = false;
        }
    }
}