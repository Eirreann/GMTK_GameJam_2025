using Game;
using GogoGaga.OptimizedRopesAndCables;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Interactions
{
    public class RopePickup : Interactable
    {
        [SerializeField] public Rope rope;
        
        public void PickupRope(bool status)
        {
            
        }

        public void Start()
        {
            
        }
        
        public override void Init(UnityAction<bool> uAction)
        {
            base.Init(uAction);
        }
    }
}