using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionConeMesh : MonoBehaviour
{
    [SerializeField] private Transform eyePoint;
    [SerializeField] private int rayCount = 30;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float coneHalfAngle = 45f;
    [SerializeField] private LayerMask groundMask;

    private Mesh mesh;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void LateUpdate()
    {
        BuildConeMesh();
    }

    private void BuildConeMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // origin at eye point
        vertices.Add(eyePoint.localPosition);

        float step = (coneHalfAngle * 2f) / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            float angle = -coneHalfAngle + step * i;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.down;

            RaycastHit2D hit = Physics2D.Raycast(eyePoint.position, dir, maxDistance, groundMask);

            Vector3 endPos = hit ? (Vector3)hit.point : eyePoint.position + (Vector3)(dir * maxDistance);

            // convert world → local
            vertices.Add(transform.InverseTransformPoint(endPos));
        }

        // build triangles
        for (int i = 1; i < rayCount; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
    }
}
