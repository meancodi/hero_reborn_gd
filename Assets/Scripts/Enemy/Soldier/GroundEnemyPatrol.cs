// ── ORIGINAL (commented out – kept for reference) ──────────────────────────
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
//        target = pointA.position;
//        fixedY = transform.position.y;
//    }

//    private void Update()
//    {
//        if (vision.playerDetected)
//            return;

//        Patrol();
//    }

//    private void Patrol()
//    {
//        Vector3 current = transform.position;
//        Vector3 targetPos = new Vector3(target.x, fixedY, current.z);

//        transform.position = Vector3.MoveTowards(current, targetPos, speed * Time.deltaTime);

//        if ((targetPos.x - current.x) > 0)
//            transform.localScale = new Vector3(realScale, realScale, realScale);
//        else if ((targetPos.x - current.x) < 0)
//            transform.localScale = new Vector3(-realScale, realScale, realScale);

//        if (Mathf.Abs(current.x - targetPos.x) < 0.1f)
//        {
//            target = (target == pointA.position) ? pointB.position : pointA.position;
//        }
//    }
//}

using UnityEngine;

/// <summary>
/// Handles both normal patrol (Waves 1 & 2) and boss follow behaviour.
/// Boss mode is AUTO-DETECTED at runtime: any enemy whose ancestor
/// is a GameObject named "Wave3" becomes a boss — no scene changes needed.
/// </summary>
public class GroundEnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Movement")]
    [SerializeField] private float speed     = 2f;
    [SerializeField] private float realScale = 1f;

    [Header("Boss Overrides (applied automatically for Wave3)")]
    [SerializeField] private float bossScale = 2.5f;
    [SerializeField] private float bossSpeed = 3.5f;

    // ── runtime ───────────────────────────────────────────────────────────
    private bool      isBoss;
    private Vector3   target;
    private float     fixedY;

    private Animator           anim;
    private GroundEnemyVisionBox vision;
    private Transform           player;

    // ─────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        anim   = GetComponent<Animator>();
        vision = GetComponent<GroundEnemyVisionBox>();
        fixedY = transform.position.y;
    }

    private void OnEnable()
    {
        // Detect boss: enemy is a child of a Wave3 GameObject AND is the chosen one
        isBoss = IsInsideWave("Wave3") && AmITheChosenBoss();

        if (isBoss)
        {
            InitBoss();
        }
        else
        {
            // Normal patrol setup
            if (pointB != null)
                target = pointB.position;

            if (anim != null)
                anim.SetBool("isPatrolling", true);
        }
    }

    private bool AmITheChosenBoss()
    {
        // Find Wave3 parent
        Transform wave3 = null;
        Transform t = transform.parent;
        while (t != null)
        {
            if (t.name == "Wave3") { wave3 = t; break; }
            t = t.parent;
        }

        if (wave3 == null) return false;

        // The first child of Wave3 (or first one with GroundEnemyPatrol) is the boss
        GroundEnemyPatrol[] enemies = wave3.GetComponentsInChildren<GroundEnemyPatrol>(true);
        if (enemies.Length > 0 && enemies[0].gameObject == gameObject)
        {
            return true;
        }

        return false;
    }

    private void OnDisable()
    {
        if (anim != null)
            anim.SetBool("isPatrolling", false);
    }

    private void Update()
    {
        if (isBoss)
        {
            // Late-find player if needed (e.g. respawn)
            if (player == null)
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p == null) return;
                player = p.transform;
                fixedY = transform.position.y;
            }

            ChasePlayer();
        }
        else
        {
            // Stop patrolling when player is spotted (attack takes over)
            if (vision != null && vision.playerDetected)
                return;

            Patrol();
        }
    }

    // ── Boss: auto-detection ──────────────────────────────────────────────

    /// <summary>Walk up the hierarchy looking for a parent named <paramref name="waveName"/>.</summary>
    private bool IsInsideWave(string waveName)
    {
        Transform t = transform.parent;
        while (t != null)
        {
            if (t.name == waveName)
                return true;
            t = t.parent;
        }
        return false;
    }

    private void InitBoss()
    {
        // Scale up — preserve facing direction (sign of X)
        float sign = transform.localScale.x >= 0f ? 1f : -1f;
        transform.localScale = new Vector3(sign * bossScale, bossScale, bossScale);
        fixedY = transform.position.y;   // recompute after scale change

        // Cache player reference
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        Debug.Log($"[Boss] {gameObject.name} is now a boss (scale={bossScale}, speed={bossSpeed})");
    }

    // ── Boss: chase behaviour ─────────────────────────────────────────────

    private void ChasePlayer()
    {
        float diff = player.position.x - transform.position.x;

        // Stop close enough for the gun to fire, but not on top of the player
        const float stopDist = 1.5f;
        if (Mathf.Abs(diff) <= stopDist)
        {
            FaceDir(diff);
            SetWalkAnim(false);
            return;
        }

        Vector3 dest = new Vector3(player.position.x, fixedY, transform.position.z);
        transform.position = Vector3.MoveTowards(
            transform.position, dest, bossSpeed * Time.deltaTime);

        FaceDir(diff);
        SetWalkAnim(true);
    }

    // ── Normal: patrol between two points ────────────────────────────────

    private void Patrol()
    {
        Vector3 pos  = transform.position;
        Vector3 dest = new Vector3(target.x, fixedY, pos.z);

        transform.position = Vector3.MoveTowards(pos, dest, speed * Time.deltaTime);

        // Face movement direction
        if (target.x > pos.x)
            transform.localScale = new Vector3( realScale, realScale, realScale);
        else
            transform.localScale = new Vector3(-realScale, realScale, realScale);

        // Swap waypoint when reached
        if (Mathf.Abs(pos.x - dest.x) < 0.05f)
            target = (target == pointA.position) ? pointB.position : pointA.position;
    }

    // ── Shared helpers ────────────────────────────────────────────────────

    private void FaceDir(float diff)
    {
        float s = Mathf.Abs(transform.localScale.x);
        transform.localScale = diff > 0f
            ? new Vector3( s, s, s)
            : new Vector3(-s, s, s);
    }

    private void SetWalkAnim(bool walking)
    {
        if (anim != null)
            anim.SetBool("isPatrolling", walking);
    }

    /// <summary>Called by external scripts to set waypoints at runtime.</summary>
    public void SetPatrolPoints(Transform a, Transform b)
    {
        pointA = a;
        pointB = b;
        target = pointB != null ? pointB.position : Vector3.zero;
    }
}
