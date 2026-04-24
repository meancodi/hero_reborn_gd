using UnityEngine;
using System.Collections;

public class RectangleFollow : MonoBehaviour
{
    private const float SPEED = 3.5f;
    private const float STOP_DISTANCE = 1.8f;
    private const float SHOOT_COOLDOWN = 3.0f; // Reduced firing speed
    private const float WARNING_TIME = 1.0f;   
    
    private const float BULLET_RANGE = 7.0f;
    private const int BULLET_COUNT = 4;
    private const float SPREAD_ANGLE = 30f;
    private const float BULLET_SPEED = 7f;

    [SerializeField] private Vector3 rectangleScale = new Vector3(2f, 3f, 1f);
    
    private Transform player;
    private Rigidbody2D rb;
    private float shootTimer;
    private SpriteRenderer visualRenderer;
    
    private bool isJumping = false;
    private bool nextAttackIsJump = false;

    private Transform healthBarPivot;
    private EnemyHealth myHealth;
    
    private GameObject hbBgObj;
    private GameObject hbPivotObj;

    private Animator animator;

    private void Awake()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p == null) p = FindObjectOfType<PlayerMovement>()?.gameObject;
        
        if (p != null) 
        {
            player = p.transform;
            transform.position = player.position + new Vector3(10f, 0f, 0f);
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f; 
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        gameObject.tag = "Enemy";

        if (GetComponent<BoxCollider2D>() == null)
        {
            BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
            col.size = new Vector2(rectangleScale.x, rectangleScale.y);
        }

        myHealth = GetComponent<EnemyHealth>();
        if (myHealth == null) myHealth = gameObject.AddComponent<EnemyHealth>();
        myHealth.SetStartingHealth(15f);

        // Screen-top Health Bar Logic
        Camera cam = Camera.main;
        if (cam != null)
        {
            float ortho = cam.orthographicSize;
            float barWidth = ortho * 1.5f;
            float barHeight = ortho * 0.15f;

            // Background
            hbBgObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            hbBgObj.transform.SetParent(cam.transform);
            hbBgObj.transform.localPosition = new Vector3(0, ortho * 0.85f, 10f); // Top of screen
            hbBgObj.transform.localScale = new Vector3(barWidth, barHeight, 1f);
            Destroy(hbBgObj.GetComponent<Collider>());
            Renderer bgRend = hbBgObj.GetComponent<Renderer>();
            bgRend.material = new Material(Shader.Find("Sprites/Default"));
            bgRend.material.color = Color.black;

            // Pivot for Left-to-Right scaling
            hbPivotObj = new GameObject("HealthBarPivot");
            hbPivotObj.transform.SetParent(cam.transform);
            hbPivotObj.transform.localPosition = new Vector3(-barWidth / 2f, ortho * 0.85f, 9.9f);
            hbPivotObj.transform.localScale = Vector3.one;
            healthBarPivot = hbPivotObj.transform;

            // Fill
            GameObject hbFill = GameObject.CreatePrimitive(PrimitiveType.Quad);
            hbFill.transform.SetParent(healthBarPivot);
            hbFill.transform.localPosition = new Vector3(barWidth / 2f, 0, 0); // Offset to center relative to pivot
            hbFill.transform.localScale = new Vector3(barWidth, barHeight, 1f);
            Destroy(hbFill.GetComponent<Collider>());
            Renderer fillRend = hbFill.GetComponent<Renderer>();
            fillRend.material = new Material(Shader.Find("Sprites/Default"));
            fillRend.material.color = Color.red; // Red for boss health
        }

        // Visual setup (Replaced Cube with SpriteRenderer + Animator)
        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = Vector3.one;
        
        visualRenderer = visual.AddComponent<SpriteRenderer>();
        visualRenderer.color = Color.white;
        visualRenderer.sortingOrder = 5;
        
        // Ensure a default sprite is loaded so it's never invisible
        Sprite[] walkSprites = Resources.LoadAll<Sprite>("Boss/boss_walk_1");
        if (walkSprites != null && walkSprites.Length > 0)
        {
            visualRenderer.sprite = walkSprites[0];
        }
        else
        {
            Debug.LogError("[RectangleFollow] Failed to load boss_walk_1 sprite from Resources/Boss!");
        }
        
        animator = visual.AddComponent<Animator>();
        RuntimeAnimatorController animCtrl = Resources.Load<RuntimeAnimatorController>("Boss/Boss");
        if (animCtrl != null)
        {
            animator.runtimeAnimatorController = animCtrl;
            animator.Play("boss_walk");
        }
        else
        {
            Debug.LogError("[RectangleFollow] Failed to load AnimatorController Boss/Boss!");
        }
    }

    private void Update()
    {
        if (myHealth != null && healthBarPivot != null)
        {
            float hpPct = myHealth.GetCurrentHealth() / myHealth.GetStartingHealth();
            healthBarPivot.localScale = new Vector3(Mathf.Clamp01(hpPct), 1f, 1f);
        }
    }

    private void OnDestroy()
    {
        if (hbBgObj != null) Destroy(hbBgObj);
        if (hbPivotObj != null) Destroy(hbPivotObj);
    }

    private void FixedUpdate()
    {
        if (player == null || isJumping) return;

        // 1. Movement
        float playerX = player.position.x;
        float myX = rb.position.x;
        float diffX = playerX - myX;
        float velX = (Mathf.Abs(diffX) > STOP_DISTANCE) ? Mathf.Sign(diffX) * SPEED : 0f;
        
        // Face player
        if (velX != 0) transform.localScale = new Vector3(Mathf.Sign(velX), 1, 1);

        rb.linearVelocity = new Vector2(velX, rb.linearVelocity.y);

        // 2. Shooting / Attacking
        shootTimer += Time.fixedDeltaTime;

        // Warning color
        if (shootTimer >= (SHOOT_COOLDOWN - WARNING_TIME))
        {
            if (visualRenderer != null) visualRenderer.color = new Color(1f, 0.5f, 0.5f); // Reddish warning tint
        }
        else
        {
            if (visualRenderer != null) visualRenderer.color = Color.white;
        }

        if (shootTimer >= SHOOT_COOLDOWN)
        {
            shootTimer = 0f;
            if (nextAttackIsJump)
            {
                StartCoroutine(JumpAttackSequence());
            }
            else
            {
                StartCoroutine(ShootSequence());
            }
            nextAttackIsJump = !nextAttackIsJump;
        }
    }

    private IEnumerator JumpAttackSequence()
    {
        isJumping = true;
        if (animator != null) animator.Play("boss_jump");
        rb.linearVelocity = Vector2.zero;
        
        Vector3 targetPos = player.position;

        // Spawn Danger Zone (Red Sphere flattened to look like a circle on the ground)
        GameObject dangerZone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dangerZone.transform.position = targetPos;
        dangerZone.transform.localScale = new Vector3(6f, 6f, 0.1f);
        Destroy(dangerZone.GetComponent<Collider>());
        
        Renderer dzRenderer = dangerZone.GetComponent<Renderer>();
        // Use a standard transparent 2D sprite shader to ensure the alpha transparency works cleanly
        dzRenderer.material = new Material(Shader.Find("Sprites/Default"));
        dzRenderer.material.color = new Color(1f, 0f, 0f, 0.15f); // Transparent red warning
        
        // Wait 0.8 seconds (Warning Phase)
        yield return new WaitForSeconds(0.8f);
        
        if (dangerZone != null) Destroy(dangerZone);

        // The Jump (Execution Phase)
        Vector3 startPos = transform.position;
        float jumpDuration = 0.5f;
        float jumpHeight = 5f;
        float elapsed = 0f;

        rb.isKinematic = true; // disable gravity during the manual arc

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;
            
            // Calculate parabolic arc
            float currentX = Mathf.Lerp(startPos.x, targetPos.x, t);
            float currentY = Mathf.Lerp(startPos.y, targetPos.y, t) + Mathf.Sin(t * Mathf.PI) * jumpHeight;
            
            transform.position = new Vector3(currentX, currentY, startPos.z);
            yield return null;
        }

        transform.position = targetPos;
        rb.isKinematic = false; // re-enable gravity
        
        // Lethality check
        if (player != null && Vector2.Distance(transform.position, player.position) <= 3f)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(9999f); // Instant death
            }
        }

        // Resume normal state
        isJumping = false;
        if (animator != null) animator.Play("boss_walk");
        if (visualRenderer != null) visualRenderer.color = Color.white;
    }

    private IEnumerator ShootSequence()
    {
        if (animator != null) animator.Play("boss_shoot");
        
        yield return new WaitForSeconds(0.2f);
        
        ShootShotgun();
        
        yield return new WaitForSeconds(0.5f);
        
        if (animator != null && !isJumping) animator.Play("boss_walk");
    }

    private void ShootShotgun()
    {
        if (player == null) return;
        
        Vector2 targetCenter = new Vector2(player.position.x, player.position.y);
        Vector2 myPos = rb.position;
        Vector2 baseDir = (targetCenter - myPos).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;
        Vector3 spawnPos = transform.position + (Vector3)baseDir * 1.5f;

        for (int i = 0; i < BULLET_COUNT; i++)
        {
            float offset = (i - (BULLET_COUNT - 1) / 2f) * (SPREAD_ANGLE / (BULLET_COUNT - 1));
            float finalAngle = (baseAngle + offset) * Mathf.Deg2Rad;
            Vector2 bulletDir = new Vector2(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle));

            GameObject bulletObj = new GameObject("ShotgunBullet");
            bulletObj.transform.position = spawnPos;
            bulletObj.AddComponent<RectangleBullet>().Initialize(bulletDir, BULLET_SPEED, BULLET_RANGE);
        }
    }
}
