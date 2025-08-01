using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class PatrolRoute : MonoBehaviour
    {
        [SerializeField] private List<Transform> _waypoints;

        private const float PATROL_WAIT_TIME = 1f;
        
        private int _currentTargetIndex = 0;
        private float _patrolWaitCooldown = 0f;

        public void Patrol(NavMeshAgent agent)
        {
            var distance = Vector3.Distance(agent.transform.position, _waypoints[_currentTargetIndex].position);
            if (distance < 0.5f)
            {
                agent.isStopped = true;
                _patrolWaitCooldown = PATROL_WAIT_TIME;
                _currentTargetIndex++;
                if (_currentTargetIndex > _waypoints.Count - 1)
                    _currentTargetIndex = 0;
            }
            else
            {
                if (_patrolWaitCooldown > 0)
                {
                    _patrolWaitCooldown -= Time.deltaTime;
                    if(_patrolWaitCooldown < 0)
                        _patrolWaitCooldown = 0;
                }
                else
                {
                    agent.isStopped = false;
                    agent.SetDestination(_waypoints[_currentTargetIndex].position);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 1);

            
            Gizmos.color = Color.yellow;
            List<Vector3> points = new List<Vector3>();
            _waypoints.ForEach(w => points.Add(w.position));
            if (points.Count > 1)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    int nextPoint = i + 1;
                    if (nextPoint > points.Count - 1)
                        nextPoint = 0;
                    Gizmos.DrawLine(points[i], points[nextPoint]);
                }
            }
        }
    }
}