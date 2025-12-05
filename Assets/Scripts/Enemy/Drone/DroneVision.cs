using UnityEngine;

public class DroneVision : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform eyePoint;

    [SerializeField] private int detectionRayCount = 30;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float coneHalfAngle = 45f;
    [SerializeField] private float graceTime = 1f;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask playerMask;

    [SerializeField] private MeshRenderer coneRenderer;
    private Material coneMat;


    [SerializeField] private float detectTime = 1.5f;

    private float detectTimer = 0f;
    private float timer = 0f;


    private void Awake()
    {
        timer = 0f;
        coneMat = coneRenderer.material;

    }


    private void Update()
    {
        if (GameManager.instance.gameOver) return;

        timer += Time.deltaTime;
        if (timer < graceTime) return;

        

        bool playerVisible = CheckConeDetection();

        if (playerVisible)
        {
            float t = detectTimer / detectTime;
            coneMat.SetFloat("_AlertAmount", t);

            detectTimer += Time.deltaTime;

            if (detectTimer >= detectTime)
            {
                print("Detected!");
               
                GameManager.instance.PlayerDefeated();
            }
        }
        else
        {
            detectTimer = 0f;
            coneMat.SetFloat("_AlertAmount", 0f);
        }
    }

    private bool CheckConeDetection()
    {
        float step = (coneHalfAngle * 2f) / (detectionRayCount - 1);

        for (int i = 0; i < detectionRayCount; i++)
        {
            float angle = -coneHalfAngle + step * i;

            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.down;

            // Cast a ray inside the cone
            RaycastHit2D hit = Physics2D.Raycast(
                eyePoint.position,
                dir,
                maxDistance,
                groundMask | playerMask
            );

            // If ray hit nothing → skip
            if (!hit.collider) continue;

            // If something else is hit first → skip
            if (!hit.collider.CompareTag("Player")) continue;

            // This ray hit the player! (and wasn't blocked)
            return true;
        }

        return false;
    }
}

