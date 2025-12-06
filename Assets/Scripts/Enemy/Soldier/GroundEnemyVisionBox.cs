//using UnityEngine;

//public class GroundEnemyVisionBox : MonoBehaviour
//{
//    [Header("Vision Settings")]
//    public Vector2 boxSize = new Vector2(4f, 1.5f);
//    public float maxDistance = 5f;
//    public LayerMask playerMask;
//    public LayerMask groundMask;

//    [Header("References")]
//    public Transform eyePoint;        // where boxcast starts

//    public bool playerDetected = false;
//    public Transform player;

//    private void Start()
//    {
//        player = GameObject.FindGameObjectWithTag("Player").transform;
//    }

//    private void Update()
//    {
//        playerDetected = CheckVision();
//    }

//    private bool CheckVision()
//    {
//        Vector2 direction = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;

//        RaycastHit2D hit = Physics2D.BoxCast(
//            eyePoint.position,
//            boxSize,
//            0f,
//            direction,
//            maxDistance,
//            playerMask | groundMask
//        );

//        // Debug visualization
//        DebugDrawBoxCast(eyePoint.position, boxSize, direction);

//        if (!hit.collider) return false;

//        if (hit.collider.CompareTag("Player"))
//            return true;

//        // Something else (ground) is blocking view
//        return false;
//    }

//    private void DebugDrawBoxCast(Vector2 origin, Vector2 size, Vector2 dir)
//    {
//        Vector2 end = origin + dir * maxDistance;
//        Color c = Color.yellow;
//        Debug.DrawLine(origin + new Vector2(size.x / 2, size.y / 2), end + new Vector2(size.x / 2, size.y / 2), c);
//        Debug.DrawLine(origin + new Vector2(size.x / 2, -size.y / 2), end + new Vector2(size.x / 2, -size.y / 2), c);
//        Debug.DrawLine(origin + new Vector2(-size.x / 2, size.y / 2), end + new Vector2(-size.x / 2, size.y / 2), c);
//        Debug.DrawLine(origin + new Vector2(-size.x / 2, -size.y / 2), end + new Vector2(-size.x / 2, -size.y / 2), c);
//    }
//}


using UnityEngine;

public class GroundEnemyVisionBox : MonoBehaviour
{
    [Header("Vision Settings")]
    public Vector2 boxSize = new Vector2(4f, 1.5f);
    public float maxDistance = 5f;
    public LayerMask playerMask;
    public LayerMask groundMask;

    [Header("References")]
    public Transform eyePoint;

    [HideInInspector] public bool playerDetected;
    [HideInInspector] public Transform player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        playerDetected = CheckVision();
    }

    private bool CheckVision()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.BoxCast(
            eyePoint.position,
            boxSize,
            0f,
            direction,
            maxDistance,
            playerMask | groundMask
        );

        return hit.collider != null && hit.collider.CompareTag("Player");
    }
}
