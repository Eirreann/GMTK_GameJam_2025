using Game;
using UnityEngine;
using Utilities;

namespace Player.PlayerStates
{
    public class FallingState : IState
    {
        private PlayerController player;

        public FallingState(PlayerController player)
        {
            this.player = player;
        }

        public void Enter()
        {
            player.playerMovement.IsGrounded = false;
        }

        private bool _hasWallJumped;
        private const float WALL_JUMP_RANGE = 3f;
        
        public void Update()
        {
            player.playerMovement.ProcessMovement(player.playerMovement.playerSpeed);
            
            player.rb.AddForce(Vector3.down * player.playerMovement.gravityScale, ForceMode.Force);
            
            //do the crouch thing
            if (GameManager.Instance.inputHandler._crouch)
            {
                player.transform.localScale = new Vector3(player.transform.localScale.x, 1f, player.transform.localScale.z);
            }
            else
            {
                player.transform.localScale = new Vector3(player.transform.localScale.x, 2f, player.transform.localScale.z);
            }
            
            //Check if player is near a wall.
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

            // Check if player has reached the ground.
            RaycastHit hit;
            if (Physics.Raycast(player.playerCamera.transform.position, Vector3.down, out hit, player.transform.localScale.y + 0.05f, player.playerMovement.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                if (GameManager.Instance.inputHandler._crouch)
                {
                    player._playerStateMachine.ChangeState(player._playerStateMachine.crouchingState);
                    return;
                }
                
                if (new Vector3(player.rb.linearVelocity.x, 0, player.rb.linearVelocity.z).magnitude > 0.1f)
                {
                    player._playerStateMachine.ChangeState(player._playerStateMachine.runningState);
                }
                else
                {
                    player._playerStateMachine.ChangeState(player._playerStateMachine.idleState);
                }
            }
        }

        public void FixedUpdate()
        {
            if (_hasWallJumped)
            {
                player.playerMovement.DoWallJump();
                
                _hasWallJumped = false;
            }
        }
        
        public void Exit()
        {
            player.canWallJump = false;
            player.lastWallJumped = null;
        }
    }
}