using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using Game;
using UnityEngine;
using Utilities;

namespace Interactions
{
    public class WallParent : MonoBehaviour
    {
        private List<WallSegment> _segments = new List<WallSegment>();

        private const float BUILD_DELAY = 0.25f;
        private const int WALL_HEALTH = 45;
        
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
            _positionWallsOnGround();
            StartCoroutine(_buildAfterDelay());
        }
        
        public void AddWallSegment(WallSegment segment)
        {
            segment.transform.SetParent(transform);
            segment.SetOnHitCallback(_onWallHit);
            segment.GetComponent<Animator>().Play("Wall_Create");
            _segments.Add(segment);
        }

        public void DestroyWall()
        {
            // TODO: destroy visual effect?
            _trappedEnemies.ForEach(e => e.SetTrapped(false));
            Destroy(gameObject);
        }

        private IEnumerator _buildAfterDelay()
        {
            yield return new WaitForSeconds(BUILD_DELAY);
            _segments.ForEach(s => s.BuildAfterDelay());
            _checkIfEnemyIsCaptured();
        }

        private void _onWallHit(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                GameManager.Instance.CurrentLevel.ReturnRope();
                GameManager.Instance.CurrentLevel.LevelWalls.Remove(this);
                DestroyWall();
            }
            
            //Debug.Log($"Wall health: {_currentHealth}/{WALL_HEALTH}");
        }

        private void _checkIfEnemyIsCaptured()
        {
            //List<GameObject> enemyObjects = GameObject.FindGameObjectsWithTag("Enemy").ToList();
            GameManager.Instance.CurrentLevel.enemies.ForEach(e =>
            {
                Vector3 enemyPosition = e.transform.position;
                if (GeometryHelper.IsPointInPolygonXZ(enemyPosition, _points.ToArray()))
                {
                    e.SetTrapped(true);
                    _trappedEnemies.Add(e);
                }
            });
        }

        private void _positionWallsOnGround()
        {
            const float pivotOffset = 0f;
            Vector3 origin = _segments[0].transform.position + Vector3.up * 0.5f;
            Ray ray = new Ray(origin, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    _segments.ForEach(s =>
                    {
                        Vector3 pos = s.transform.position;
                        pos.y = hit.point.y + pivotOffset;
                        s.transform.position = pos;
                        
                    });
                }
            }
        }
    }
}