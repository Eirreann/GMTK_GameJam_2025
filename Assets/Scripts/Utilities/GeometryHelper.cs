using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class GeometryHelper : MonoBehaviour
    {
        private const float DefaultMinPointDistance = 0.01f; // tweak as needed (units)
        private const float ColinearAngleDeg = 5f; // remove a point if the angle between segments is < this
        
        // Returns true if point is inside polygon (on XZ plane)
        public static bool IsPointInPolygonXZ(Vector3 point, Vector3[] polygon)
        {
            int crossings = 0;
            Vector2 p = new Vector2(point.x, point.z);

            for (int i = 0; i < polygon.Length; i++)
            {
                Vector2 a = new Vector2(polygon[i].x, polygon[i].z);
                Vector2 b = new Vector2(polygon[(i + 1) % polygon.Length].x, polygon[(i + 1) % polygon.Length].z);

                if (((a.y > p.y) != (b.y > p.y)) &&
                    (p.x < (b.x - a.x) * (p.y - a.y) / (b.y - a.y + float.Epsilon) + a.x))
                {
                    crossings++;
                }
            }

            return (crossings % 2) == 1;
        }
        
        // Returns true if the 2D segments p1-p2 and q1-q2 intersect. If true, 'intersection' is set.
        public static bool LineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2, out Vector2 intersection)
        {
            intersection = Vector2.zero;

            float A1 = p2.y - p1.y;
            float B1 = p1.x - p2.x;
            float C1 = A1 * p1.x + B1 * p1.y;

            float A2 = q2.y - q1.y;
            float B2 = q1.x - q2.x;
            float C2 = A2 * q1.x + B2 * q1.y;

            float denom = A1 * B2 - A2 * B1;

            if (Mathf.Abs(denom) < Mathf.Epsilon)
                return false; // Parallel or coincident

            float ix = (B2 * C1 - B1 * C2) / denom;
            float iy = (A1 * C2 - A2 * C1) / denom;
            Vector2 r = new Vector2(ix, iy);

            if (IsPointOnSegment(p1, p2, r) && IsPointOnSegment(q1, q2, r))
            {
                intersection = r;
                return true;
            }

            return false;
        }

        private static bool IsPointOnSegment(Vector2 a, Vector2 b, Vector2 p)
        {
            float minX = Mathf.Min(a.x, b.x) - Mathf.Epsilon;
            float maxX = Mathf.Max(a.x, b.x) + Mathf.Epsilon;
            float minY = Mathf.Min(a.y, b.y) - Mathf.Epsilon;
            float maxY = Mathf.Max(a.y, b.y) + Mathf.Epsilon;
            return p.x >= minX && p.x <= maxX && p.y >= minY && p.y <= maxY;
        }
        
        public static List<Vector3> CleanLoopPoints(List<Vector3> points, float minDist = DefaultMinPointDistance, float colinearAngleDeg = ColinearAngleDeg)
        {
            if (points == null || points.Count <= 1) return points;

            float minDistSqr = minDist * minDist;
            var cleaned = new List<Vector3>(points.Count);
            cleaned.Add(points[0]);
            for (int i = 1; i < points.Count; i++)
            {
                if ((points[i] - cleaned[cleaned.Count - 1]).sqrMagnitude > minDistSqr)
                    cleaned.Add(points[i]);
            }

            if (cleaned.Count >= 2 && (cleaned[0] - cleaned[cleaned.Count - 1]).sqrMagnitude < minDistSqr)
            {
                cleaned.RemoveAt(cleaned.Count - 1);
            }

            if (cleaned.Count >= 3)
            {
                var simplified = new List<Vector3>(cleaned.Count);
                for (int i = 0; i < cleaned.Count; i++)
                {
                    Vector3 prev = simplified.Count == 0 ? cleaned[(i - 1 + cleaned.Count) % cleaned.Count] : simplified[simplified.Count - 1];
                    Vector3 curr = cleaned[i];
                    Vector3 next = cleaned[(i + 1) % cleaned.Count];

                    Vector3 v1 = (curr - prev).normalized;
                    Vector3 v2 = (next - curr).normalized;

                    float angle = Vector3.Angle(v1, v2);
                    if (angle <= colinearAngleDeg && (curr - prev).sqrMagnitude < ( (next - curr).sqrMagnitude + minDistSqr ) )
                    {
                        continue;
                    }

                    simplified.Add(curr);
                }

                if (simplified.Count >= 2 && (simplified[0] - simplified[simplified.Count - 1]).sqrMagnitude < minDistSqr)
                    simplified.RemoveAt(simplified.Count - 1);

                return simplified;
            }

            return cleaned;
        }

        // Debug helper: print any very small segments for tuning
        public static void LogTinySegments(IReadOnlyList<Vector3> pts, float tinyThreshold = 0.001f)
        {
            float tSqr = tinyThreshold * tinyThreshold;
            for (int i = 0; i < pts.Count; i++)
            {
                Vector3 a = pts[i];
                Vector3 b = pts[(i + 1) % pts.Count];
                float d2 = (b - a).sqrMagnitude;
                if (d2 < tSqr)
                    Debug.LogWarning($"Tiny segment {i} -> {(i+1)%pts.Count}, length={Mathf.Sqrt(d2):0.00000f}");
            }
        }
    }
}