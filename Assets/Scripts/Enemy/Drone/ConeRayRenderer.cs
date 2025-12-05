using UnityEngine;
using System.Collections.Generic;

public class ConeRayRenderer : MonoBehaviour
{
    [SerializeField] private Transform eyePoint;
    [SerializeField] private int rayCount = 30;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float coneHalfAngle = 45f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private GameObject rayPrefab;

    private List<LineRenderer> rays = new List<LineRenderer>();

    private void Start()
    {
        // Create ray renderers
        for (int i = 0; i < rayCount; i++)
        {
            GameObject rayObj = Instantiate(rayPrefab, transform);
            LineRenderer lr = rayObj.GetComponent<LineRenderer>();


            rays.Add(lr);
        }
    }

    private void Update()
    {
        DrawRays();
    }

    private void DrawRays()
    {
        float step = (coneHalfAngle * 2f) / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            float angle = -coneHalfAngle + (step * i);

            // Direction of this ray
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.down;

            // Raycast for obstruction
            RaycastHit2D hit = Physics2D.Raycast(eyePoint.position, dir, maxDistance, groundMask);

            Vector3 endPos;

            if (hit)
                endPos = hit.point; // blocked by platform
            else
                endPos = eyePoint.position + (Vector3)(dir * maxDistance);

            // Update LineRenderer positions
            rays[i].SetPosition(0, eyePoint.position);
            rays[i].SetPosition(1, endPos);
        }
    }
}
