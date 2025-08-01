using GogoGaga.OptimizedRopesAndCables;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerInteractions : MonoBehaviour
    {
        public Rope rope;
        public GameObject depositPoint;

        public TextMeshProUGUI text;

        public bool holding_rope = false;
        private const float INTERACT_DISTANCE = 3f;
        
        public void Update()
        {
            if (GameManager.Instance.inputHandler._interact)
            {
                var ropeDis = Vector3.Distance(transform.position, rope.transform.position);
                if (ropeDis < INTERACT_DISTANCE && !holding_rope)
                {
                    rope.endPoint = transform;
                    holding_rope = true;
                }
                
                if (holding_rope)
                {
                    var depositDis = Vector3.Distance(transform.position, depositPoint.transform.position);
                    if (depositDis < INTERACT_DISTANCE)
                    {
                        rope.endPoint = depositPoint.transform;
                        holding_rope = false;

                        text.text = "U r win. Congrats.";
                    }
                }
            }
        }
    }
}