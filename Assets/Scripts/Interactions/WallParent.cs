using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;
using Utilities;

namespace Interactions
{
    public class WallParent : MonoBehaviour
    {
        private List<WallSegment> _segments = new List<WallSegment>();

        private const int WALL_HEALTH = 40;
        
        private Vector3[] _points;
        private List<Enemy_Base> _trappedEnemies = new List<Enemy_Base>();
        private int _currentHealth;

        private void Awake()
        {
            _currentHealth = WALL_HEALTH;
        }

        public void Init(Vector3[] points)
        {
            _points = points;
            StartCoroutine(_buildAfterDelay());
        }
        
        public void AddWallSegment(WallSegment segment)
        {
            segment.transform.SetParent(transform);
            segment.SetOnHitCallback(_onWallHit);
            _segments.Add(segment);
        }

        public void DestroyWall()
        {
            // TODO: destroy visual effect?

            GameManager.Instance.CurrentLevel.LevelWalls.Remove(this);
            _trappedEnemies.ForEach(e => e.SetTrapped(false));
            Destroy(gameObject);
        }

        private IEnumerator _buildAfterDelay()
        {
            yield return new WaitForSeconds(.8f);
            _segments.ForEach(s => s.BuildAfterDelay());
            _checkIfEnemyIsCaptured();
        }

        private void _onWallHit(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                GameManager.Instance.CurrentLevel.ReturnRope();
                
                DestroyWall();
            }
            
            //Debug.Log($"Wall health: {_currentHealth}/{WALL_HEALTH}");
        }

        private void _checkIfEnemyIsCaptured()
        {
            List<GameObject> enemyObjects = GameObject.FindGameObjectsWithTag("Enemy").ToList();
            enemyObjects.ForEach(e =>
            {
                Vector3 enemyPosition = e.transform.position;
                if (GeometryHelper.IsPointInPolygonXZ(enemyPosition, _points.ToArray()))
                {
                    var enemy = e.GetComponent<Enemy_Base>();
                    enemy.SetTrapped(true);
                    _trappedEnemies.Add(enemy);
                }
            });
        }
    }
}