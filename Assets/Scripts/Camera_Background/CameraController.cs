using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("Follow Options")]
    [SerializeField] private bool followHorizontal = true;
    [SerializeField] private bool followVertical = true;

    [Header("Horizontal Follow Settings")]
    [SerializeField] private float aheadDistance = 2f;
    [SerializeField] private float cameraSpeed = 3f;
    [SerializeField] private float leftBound = -50f;
    [SerializeField] private float rightBound = 50f;

    [Header("Vertical Follow Settings")]
    [SerializeField] private float yOffset = 1f;          // how high above the player the camera stays
    [SerializeField] private float bottomLimit = -3f;      // camera Y minimum
    [SerializeField] private float topLimit = 999f;        // camera Y maximum if needed

    [Header("BackgroundColor")]
    [SerializeField] private float X = 93f / 255f;
    [SerializeField] private float Y = 127f / 255f;
    [SerializeField] private float Z = 157f / 255f;

    private float lookAhead = 0f;

    private void Awake()
    {
        Camera.main.backgroundColor = new Color(X, Y, Z);
    }

    private void LateUpdate()
    {
        if (!player) return;

        float targetX = transform.position.x;
        float targetY = transform.position.y;

        // -----------------------------
        // HORIZONTAL CAMERA FOLLOW
        // -----------------------------
        if (followHorizontal)
        {
            // Only follow X if inside horizontal bounds
            if (player.position.x > leftBound && player.position.x < rightBound)
                targetX = player.position.x + lookAhead;
        }

        // -----------------------------
        // VERTICAL CAMERA FOLLOW
        // -----------------------------
        if (followVertical)
        {
            // Compute the camera's intended vertical position
            float desiredY = player.position.y + yOffset;

            // Clamp camera Y (THIS fixes your problem)
            desiredY = Mathf.Clamp(desiredY, bottomLimit, topLimit);

            targetY = desiredY;
        }

        // -----------------------------
        // APPLY MOVEMENT
        // -----------------------------
        transform.position = new Vector3(targetX, targetY, transform.position.z);

        // Smooth horizontal look-ahead movement
        lookAhead = Mathf.Lerp(lookAhead, aheadDistance * Mathf.Sign(player.localScale.x), Time.deltaTime * cameraSpeed);
    }
}
