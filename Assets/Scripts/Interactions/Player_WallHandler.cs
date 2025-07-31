using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player_WallHandler : MonoBehaviour
{
    [SerializeField] private GameObject _wallPrefab;
    
    [Header("Trail")]
    [SerializeField] private GameObject _trailPrefab;
    [SerializeField] private float _trailDuration = 5f;
    
    private TrailRenderer _trail;
    private float _distanceThreshold = 1f;
    private bool _isDrawing = false;

    private void Update()
    {
        if (GameManager.Instance.inputHandler._drawWall && !_isDrawing)
        {
            _drawWall(true);
        }
        else if (!GameManager.Instance.inputHandler._drawWall && _isDrawing)
        {
            _drawWall(false);
        }
    }

    private void _drawWall(bool state)
    {
        _isDrawing = state;
        if (state)
        {
            _trail = Instantiate(_trailPrefab, transform).GetComponent<TrailRenderer>();
            _trail.time = _trailDuration;
            _trail.gameObject.SetActive(true);
        }
        else
        {
            var positions = new Vector3[_trail.positionCount];
            var count = _trail.GetPositions(positions);
            if (_trail.positionCount > 0)
            {
                var firstPoint = positions[0];
                if (Vector3.Distance(_trail.transform.position, firstPoint) <= _distanceThreshold)
                {
                    _buildWall(positions);
                }
                else
                {
                    // TODO: Fizzle visual/audio effect?
                }
            }
            Destroy(_trail.gameObject);
        }
    }

    private void _buildWall(Vector3[] points)
    {
        Debug.Log($"Build wall with {points.Length} corners");
        
        int count = points.Length;
        if (count < 2) return;

        Transform wallParent = new GameObject("Wall").transform;
        for (int i = 0; i < count; i++)
        {
            Vector3 start = points[i];
            Vector3 end = points[(i + 1) % count];

            Vector3 direction = end - start;
            float distance = direction.magnitude;
            Vector3 midPoint = start + direction / 2;

            var wallSegment = Instantiate(_wallPrefab, midPoint, Quaternion.identity);
            wallSegment.transform.rotation = Quaternion.LookRotation(direction);
            
            Vector3 localScale = wallSegment.transform.localScale;
            localScale.z = distance;
            wallSegment.transform.localScale = localScale;
            
            wallSegment.transform.SetParent(wallParent);
        }
    }
}
