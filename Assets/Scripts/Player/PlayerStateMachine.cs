using System;
using Utilities;
using Player.PlayerStates;

namespace Player
{
    [Serializable]
    public class PlayerStateMachine
    {
        public IState CurrentState { get; set; }
        
        public IdleState idleState { get; set; }
        public RunningState runningState { get; set; }
        public CrouchingState crouchingState { get; set; }
        public SlidingState slidingState { get; set; }
        public JumpingState jumpingState { get; set; }
        public FallingState fallingState { get; set; }
        public PausedState pausedState { get; set; }

        
        public PlayerStateMachine(PlayerController player)
        {
            this.idleState = new IdleState(player);
            this.runningState = new RunningState(player);
            this.crouchingState = new CrouchingState(player);
            this.slidingState = new SlidingState(player);
            this.jumpingState = new JumpingState(player);
            this.fallingState = new FallingState(player);
            this.pausedState = new PausedState(player);
        }

        public void Initialize(IState beginState)
        {
            CurrentState = beginState;
            beginState.Enter();
        }

        public void ChangeState(IState newState)
        {
            CurrentState.Exit();
            
            CurrentState = newState;
            
            CurrentState.Enter();
        }

        public void Update()
        {
            if (CurrentState != null)
            {
                CurrentState.Update();
            }
        }
        
        public void FixedUpdate()
        {
            if (CurrentState != null)
            {
                CurrentState.FixedUpdate();
            }
        }
    }
}