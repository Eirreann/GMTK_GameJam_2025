using Game;
using UnityEngine;

namespace Interactions
{
    public class EndDoorTrigger : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
            GameManager.Instance.ProgressToNextLevel();
        }
    }
}