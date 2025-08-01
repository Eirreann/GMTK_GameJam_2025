using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class WallParent : MonoBehaviour
    {
        private List<WallSegment> _segments = new List<WallSegment>();

        private const int WALL_HEALTH = 25;
        
        private int _currentHealth;

        private void Awake()
        {
            _currentHealth = WALL_HEALTH;
        }

        public void AddWallSegment(WallSegment segment)
        {
            segment.transform.SetParent(transform);
            segment.SetOnHitCallback(_onWallHit);
            _segments.Add(segment);
        }

        private void _onWallHit(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                // TODO: destroy visual effect?
                Destroy(gameObject);
            }
            // Debug.Log($"Wall health: {_currentHealth}/{WALL_HEALTH}");
        }
    }
}