using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineColliderGenerator : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private MeshCollider meshCollider;
    private Mesh mesh;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        
        mesh = new Mesh();
        meshCollider.convex = true; // Set to true for non-convex shapes, which can be useful for lines.
        meshCollider.sharedMesh = mesh;
    }

    void Update()
    {
        // This is a simple update check. For performance, you might want to call
        // this method only when the line's points actually change.
        GenerateColliderMesh();
    }

    private void GenerateColliderMesh()
    {
        // Get the positions of the line's vertices
        Vector3[] linePositions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(linePositions);

        // Clear previous mesh data
        mesh.Clear();

        // Check if there are enough points to create a mesh
        if (linePositions.Length < 2)
        {
            return;
        }

        // Create the vertices for the collider mesh
        Vector3[] vertices = new Vector3[linePositions.Length * 2];
        int[] triangles = new int[(linePositions.Length - 1) * 6];

        float lineWidth = lineRenderer.startWidth;

        for (int i = 0; i < linePositions.Length; i++)
        {
            Vector3 point = linePositions[i];
            Vector3 direction = Vector3.forward;

            if (i < linePositions.Length - 1)
            {
                direction = (linePositions[i + 1] - point).normalized;
            }
            else if (i > 0)
            {
                direction = (point - linePositions[i - 1]).normalized;
            }

            // Calculate a perpendicular vector for the line's width
            Vector3 crossProduct = Vector3.Cross(direction, Vector3.up);
            Vector3 perpendicular = crossProduct.normalized * lineWidth / 2f;

            // Define two vertices for each line point, offset by the width
            vertices[i * 2] = point + perpendicular;
            vertices[i * 2 + 1] = point - perpendicular;

            // Define the triangles for the quad between this point and the next
            if (i < linePositions.Length - 1)
            {
                int startIndex = i * 2;
                // Triangle 1
                triangles[i * 6] = startIndex;
                triangles[i * 6 + 1] = startIndex + 1;
                triangles[i * 6 + 2] = startIndex + 3;

                // Triangle 2
                triangles[i * 6 + 3] = startIndex;
                triangles[i * 6 + 4] = startIndex + 3;
                triangles[i * 6 + 5] = startIndex + 2;
            }
        }

        // Assign the new data to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // Recommended for proper lighting and culling
        mesh.RecalculateBounds();  // For culling and physics optimization

        // Update the mesh collider
        meshCollider.sharedMesh = mesh;
    }
}