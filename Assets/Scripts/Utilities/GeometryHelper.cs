using UnityEngine;

namespace Utilities
{
    public class GeometryHelper : MonoBehaviour
    {
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
    }
}