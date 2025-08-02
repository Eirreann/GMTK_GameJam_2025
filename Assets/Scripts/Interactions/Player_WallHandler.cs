using System;
using System.Collections.Generic;
using Interactions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Game;

public class Player_WallHandler : MonoBehaviour
{
    [SerializeField] private WallParent _wallParentPrefab;
    [SerializeField] private WallSegment _wallPrefab;
    
    [Header("Trail")]
    [SerializeField] private GameObject _trailPrefab;
    [SerializeField] private float _trailDuration = 5f;
    
    private const float COOLDOWN = 1f;
    private const float DISTANCE_THRESHOLD = 2f;
    
    private TrailRenderer _trail;
    private float _drawCooldown = 0f;
    private bool _isDrawing = false;
    private int _currentPoints = 0;

    public void ResetWalls()
    {
        if (_isDrawing)
        {
            _drawWall(false);
            _drawCooldown = COOLDOWN;
        }
    }

    private void Update()
    {
        var drawInput = _isDrawingAllowed();
        
        if (drawInput && !_isDrawing)
            _drawWall(true);
        else if (!drawInput && _isDrawing)
            _drawWall(false);

        if (_isDrawing)
            _deductDrawJuice();
    }

    private bool _isDrawingAllowed()
    {
        if(_drawCooldown > 0f)
            _drawCooldown -= Time.deltaTime;
        
        return _drawCooldown > 0f ? false : GameManager.Instance.inputHandler._drawWall;
    }

    private void _deductDrawJuice() // TODO: Refactor to better handle running out of juice
    {
        var trailLength = _trail.positionCount;
        if (trailLength != _currentPoints)
        {
            if (trailLength < _currentPoints)
            {
                GameManager.Instance.Player.playerStats.InjectJuice(_currentPoints - trailLength);
                _currentPoints = trailLength;
                return;
            }
                
            var amountToDeduct = trailLength - _currentPoints;
            _currentPoints = trailLength;
            
            var juiceRemaining = GameManager.Instance.Player.playerStats.DrainJuice(amountToDeduct);
            if (juiceRemaining <= 0)
            {
                _drawCooldown = COOLDOWN;
                _drawWall(false);
            }
        }
    }

    private void _drawWall(bool state)
    {
        _isDrawing = state;
        if (state)
        {
            _trail = Instantiate(_trailPrefab, transform).GetComponent<TrailRenderer>();
            _trail.time = _trailDuration;
        }
        else
        {
            var positions = new Vector3[_trail.positionCount];
            if (_trail.positionCount > 0)
            {
                _trail.GetPositions(positions);
                var firstPoint = positions[0];
                var distance = Vector3.Distance(_trail.transform.position, firstPoint);
                if (positions.Length > 2 && distance <= DISTANCE_THRESHOLD)
                {
                    _buildWall(positions);
                }
                else
                {
                    // TODO: Fizzle visual/audio effect?
                    
                    // Give juice back
                    GameManager.Instance.Player.playerStats.InjectJuice(_currentPoints);
                }
            }
            
            Destroy(_trail.gameObject);
            _currentPoints = 0;
        }
    }

    private void _buildWall(Vector3[] points)
    {
        int count = points.Length;
        if (count < 2) return;

        WallParent wallParent = Instantiate(_wallParentPrefab);
        for (int i = 0; i < count; i++)
        {
            Vector3 start = new Vector3(points[i].x, 1, points[i].z);
            Vector3 endPoint = points[(i + 1) % count];
            Vector3 end = new Vector3(endPoint.x, 1, endPoint.z);

            Vector3 direction = end - start;
            float distance = direction.magnitude;
            Vector3 midPoint = start + direction / 2;

            var wallSegment = Instantiate(_wallPrefab, midPoint, Quaternion.identity);
            wallSegment.transform.rotation = Quaternion.LookRotation(direction);
            
            Vector3 localScale = wallSegment.transform.localScale;
            localScale.z = distance;
            wallSegment.transform.localScale = localScale;
            
            wallParent.AddWallSegment(wallSegment);
        }
        wallParent.Init(points);
        GameManager.Instance.CurrentLevel.LevelWalls.Add(wallParent);
    }
}
