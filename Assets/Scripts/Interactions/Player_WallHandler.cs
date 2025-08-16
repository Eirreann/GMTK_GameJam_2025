using System;
using System.Collections.Generic;
using System.Numerics;
using Interactions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Game;
using Utilities;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player_WallHandler : MonoBehaviour
{
    public bool IsDrawing => _isDrawing;

    [SerializeField] private WallParent _wallParentPrefab;
    [SerializeField] private WallSegment _wallPrefab;
    
    [Header("Trail")]
    [SerializeField] private GameObject _trailPrefab;
    [SerializeField] private float _trailDuration = 5f;
    
    private const float COOLDOWN = 1f;
    private const float DISTANCE_THRESHOLD = 2f;
    private const float MIN_VERTEX_DISTANCE = 2f;
    
    private TrailRenderer _trail;
    private float _drawCooldown = 0f;
    private bool _isDrawing = false;
    private int _currentPoints = 0;
    private float _trailYPos = 0f;

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

        if (_isDrawing && _trail != null)
        {
            var trailPos = new Vector3(transform.position.x, _trailYPos, transform.position.z);
            _trail.transform.position = trailPos;
        }

        if (_isDrawing)
            _deductDrawJuice();
    }

    private bool _isDrawingAllowed()
    {
        if(_drawCooldown > 0f)
            _drawCooldown -= Time.deltaTime;
        
        return _drawCooldown > 0f ? false : GameManager.Instance.inputHandler._drawWall;
    }

    private void _deductDrawJuice()
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
            _trail = Instantiate(_trailPrefab).GetComponent<TrailRenderer>();
            _trailYPos = transform.position.y + 1f;
            _trail.time = _trailDuration;
        }
        else
        {
            // var positions = new Vector3[_trail.positionCount];
            // if (_trail.positionCount > 0)
            // {
            //     _trail.GetPositions(positions);
            //     var firstPoint = positions[0];
            //     var distance = Vector3.Distance(_trail.transform.position, firstPoint);
            //     if (positions.Length > 2 && distance <= DISTANCE_THRESHOLD)
            //     {
            //         _buildWall(positions);
            //     }
            //     else
            //     {
            //         // TODO: Fizzle visual/audio effect?
            //         
            //         // Give juice back
            //         GameManager.Instance.Player.playerStats.InjectJuice(_currentPoints);
            //     }
            // }
            
            var positions = new Vector3[_trail.positionCount];
            var count = _trail.GetPositions(positions);

            if (count >= 4) // If the line has at least four vertices (to prevent undesired small walls)
            {
                // project to XZ 2D for intersection tests
                Vector2[] path2D = new Vector2[count];
                for(int i = 0; i < count; i++)
                    path2D[i] = new Vector2(positions[i].x, positions[i].z);

                int foundI = -1; // second index of first segment (segment A is (foundI-1, foundI))
                int foundJ = -1; // first index of second segment (segment B is (foundJ, foundJ+1))
                Vector2 intersectionPoint = Vector2.zero;
                
                // find first non-adjacent intersection
                for (int i = 1; i < count - 2; i++)
                {
                    for (int j = i + 2; j < count - 1; j++) // j = i+2 ensures non-adjacent (no shared vertex)
                    {
                        if (GeometryHelper.LineSegmentsIntersect(path2D[i - 1], path2D[i], path2D[j], path2D[j + 1], out Vector2 inter))
                        {
                            foundI = i;
                            foundJ = j;
                            intersectionPoint = inter;
                            break;
                        }
                    }
                    if (foundI != -1) break;
                }

                if (foundI != -1)
                {
                    // Compute exact 3D intersection for segment A (between positions[foundI-1] and positions[foundI])
                    Vector2 a1 = path2D[foundI - 1];
                    Vector2 a2 = path2D[foundI];
                    Vector2 dirA = a2 - a1;
                    float denomA = Vector2.Dot(dirA, dirA);
                    float tA = denomA > Mathf.Epsilon ? Vector2.Dot(intersectionPoint - a1, dirA) / denomA : 0f;
                    tA = Mathf.Clamp01(tA);
                    float  yA = Mathf.Lerp(positions[foundI - 1].y, positions[foundI].y, tA);
                    Vector3 intersectionA3D = new Vector3(intersectionPoint.x, yA, intersectionPoint.y);
                    
                    // Compute exact 3D intersection for segment B (between positions[foundJ] and positions[foundJ+1])
                    Vector2 b1 = path2D[foundJ];
                    Vector2 b2 = path2D[foundJ + 1];
                    Vector2 dirB = b2 - b1;
                    float denomB = Vector2.Dot(dirB, dirB);
                    float tB = denomB > Mathf.Epsilon ? Vector2.Dot(intersectionPoint - b1, dirB) / denomB : 0f;
                    tB = Mathf.Clamp01(tB);
                    float yB = Mathf.Lerp(positions[foundJ].y, positions[foundJ + 1].y, tB);
                    Vector3 intersectionB3D = new Vector3(intersectionPoint.x, yB, intersectionPoint.y);
                    
                    // Build ordered loop: intersectionA -> positions[foundI] .. positions[foundJ] -> intersectionB
                    var loopPoints = new List<Vector3>((foundJ - foundI) + 3);
                    loopPoints.Add(intersectionA3D);
                    
                    // Add the sampled points between the two intersection segments
                    for(int k = foundI; k <= foundJ; k++)
                        loopPoints.Add(positions[k]);
                    
                    loopPoints.Add(intersectionB3D);
                    
                    // Clean near-duplicate consecutive points
                    var cleaned = GeometryHelper.CleanLoopPoints(loopPoints);
                    GeometryHelper.LogTinySegments(cleaned);
                    
                    // If after cleaning we have at least 3 vertices, build the wall
                    if (loopPoints.Count >= 3)
                    {
                        _buildWall(cleaned.ToArray());
                        
                        // Restore juice not used in the wall
                        if((_currentPoints - loopPoints.Count) > 0)
                            GameManager.Instance.Player.playerStats.InjectJuice(_currentPoints - loopPoints.Count);
                    }
                    else
                    {
                        Debug.Log("Detected intersection but loop has insufficient points after cleaning.");
                        
                    }
                }
                else
                {
                    Debug.Log("No self-intersection detected; trail ended without forming a loop.");

                    // Give juice back
                    GameManager.Instance.Player.playerStats.InjectJuice(_currentPoints);
                }
            }
            else
            {
                Debug.Log("Trail too short to form a loop.");

                // Give juice back
                GameManager.Instance.Player.playerStats.InjectJuice(_currentPoints);
                // TODO: Fizzle visual/audio effect?
            }
            
            Destroy(_trail.gameObject);
            _trail = null;
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
            float yPos = points[0].y + 1.5f;
            Vector3 start = new Vector3(points[i].x, yPos, points[i].z);
            Vector3 endPoint = points[(i + 1) % count];
            Vector3 end = new Vector3(endPoint.x, yPos, endPoint.z);

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
