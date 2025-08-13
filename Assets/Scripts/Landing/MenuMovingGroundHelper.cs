using System.Collections.Generic;
using UnityEngine;

namespace Landing
{
    public class MenuMovingGroundHelper : MonoBehaviour
    {
        [SerializeField] private float ScrollSpeed;
        [SerializeField] private float XThreshold;

        public List<Transform> children;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        void Start()
        {
            for(int i = 0, count = transform.childCount; i < count; i++)
            {
                children.Add(transform.GetChild(i));
            }
        }
    
        void Update()
        {
            foreach (Transform child in children)
            {
                CheckChildPosition(child);
            }
        }

        void CheckChildPosition(Transform child)
        {
            child.transform.Translate(new Vector3(ScrollSpeed * Time.deltaTime, 0, 0));
            if(child.transform.position.x < XThreshold) child.transform.Translate(new Vector3(child.transform.localScale.x * children.Count, 0, 0));
        }
    }
}
