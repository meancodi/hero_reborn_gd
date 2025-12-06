//using UnityEngine;

//public class GroundEnemyPatrol : MonoBehaviour
//{
//    [SerializeField] private Transform pointA;
//    [SerializeField] private Transform pointB;
//    [SerializeField] private float speed = 2f;
//    [SerializeField] private float realScale = 0.54038f;

//    private Vector3 target;
//    private GroundEnemyVisionBox vision;
//    private float fixedY;

//    private void Start()
//    {
//        vision = GetComponent<GroundEnemyVisionBox>();

//        // Start moving toward point A
//        target = pointA.position;

//        // Remember ground height so enemy never floats up/down
//        fixedY = transform.position.y;
//    }

//    private void Update()
//    {
//        // If enemy sees the player → stop and aim
//        if (vision.playerDetected)
//            return;

//        Patrol();
//    }

//    private void Patrol()
//    {
//        // Enemy should only move horizontally, never vertically
//        Vector3 current = transform.position;
//        Vector3 targetPos = new Vector3(target.x, fixedY, current.z);

//        // Move enemy
//        transform.position = Vector3.MoveTowards(current, targetPos, speed * Time.deltaTime);

//        // Flip based on direction
//        if ((targetPos.x - current.x) > 0)
//            transform.localScale = new Vector3(realScale, realScale, realScale);   // facing right
//        else if ((targetPos.x - current.x) < 0)
//            transform.localScale = new Vector3(-realScale, realScale, realScale);  // facing left

//        // If reached target, switch to the other one
//        if (Mathf.Abs(current.x - targetPos.x) < 0.1f)
//        {
//            target = (target == pointA.position) ? pointB.position : pointA.position;
//        }
//    }
//}

using UnityEngine;

public class GroundEnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float realScale = 1f;

    private Vector3 target;
    private float fixedY;

    private Animator anim;
    private GroundEnemyVisionBox vision;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        vision = GetComponent<GroundEnemyVisionBox>();

        fixedY = transform.position.y;
        target = pointB.position;
    }

    private void OnEnable()
    {
        // Patrol enabled → allow walk animation
        if (anim != null)
            anim.SetBool("isPatrolling", true);
    }

    private void OnDisable()
    {
        // Patrol disabled → force idle animation
        if (anim != null)
            anim.SetBool("isPatrolling", false);
    }

    private void Update()
    {
        // If player detected, patrol halts (aiming / firing handled elsewhere)
        if (vision != null && vision.playerDetected)
            return;

        Patrol();
    }

    private void Patrol()
    {
        Vector3 position = transform.position;
        Vector3 targetPos = new Vector3(target.x, fixedY, position.z);

        transform.position = Vector3.MoveTowards(
            position,
            targetPos,
            speed * Time.deltaTime
        );

        // Face movement direction
        if (target.x > position.x)
            transform.localScale = new Vector3(realScale, realScale, realScale);
        else
            transform.localScale = new Vector3(-realScale, realScale, realScale);

        // Swap target when reached
        if (Mathf.Abs(position.x - targetPos.x) < 0.05f)
        {
            target = (target == pointA.position)
                ? pointB.position
                : pointA.position;
        }
    }

    public void SetPatrolPoints(Transform a, Transform b)
    {
        pointA = a;
        pointB = b;
        target = pointB.position;
    }


}

