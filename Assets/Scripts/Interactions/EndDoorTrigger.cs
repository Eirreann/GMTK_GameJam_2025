using Game;
using UnityEngine;

namespace Interactions
{
    public class EndDoorTrigger : MonoBehaviour
    {
        private bool triggered = false;
        public void OnTriggerEnter(Collider other)
        {
            if (!triggered)
            {
                GameManager.Instance.ProgressToNextLevel();
                triggered = true;
            }
        }
    }
}