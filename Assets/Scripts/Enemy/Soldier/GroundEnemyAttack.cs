using UnityEngine;

public class GroundEnemyAttack : MonoBehaviour
{
    [SerializeField] private float shootCooldown = 1f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Boss Shotgun Settings")]
    [SerializeField] private int shotgunBulletCount = 5;
    [SerializeField] private float shotgunSpreadAngle = 30f;
    [SerializeField] private float shotgunBulletSpeed = 12f;

    private Animator anim;
    private GroundEnemyVisionBox vision;
    private float cooldownTimer;
    private bool isBoss;
    private SpriteRenderer sr;

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 40;
        style.normal.textColor = Color.magenta;
        GUI.Label(new Rect(50, 150, 800, 100), "CURRENT FIRE RATE: " + shootCooldown + "s", style);
    }

    private void Awake()
    {
        vision = GetComponent<GroundEnemyVisionBox>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        
        // FORCING 6 SECONDS FOR EVERYONE
        shootCooldown = 6.0f;
    }

    private void Update()
    {
        if (vision == null) return;
        
        cooldownTimer += Time.deltaTime;

        if (anim != null) anim.SetBool("isDetected", vision.playerDetected);

        if (!vision.playerDetected)
        {
            if (sr != null) sr.color = Color.white;
            return;
        }

        // VISUAL WARNING: Turn Magenta when about to shoot
        if (cooldownTimer >= (shootCooldown - 1.5f))
        {
            if (sr != null) sr.color = Color.magenta;
        }
        else
        {
            if (sr != null) sr.color = Color.white;
        }

        if (cooldownTimer >= shootCooldown)
        {
            if (anim != null) anim.SetTrigger("fire");
            Shoot();
            cooldownTimer = 0f;
        }
    }

    private void Shoot()
    {
        // For diagnostic, we just do shotgun for everyone
        ShootShotgun();
    }

    private void ShootNormal()
    {
        GameObject bullet = BulletPool.instance.GetBullet();
        bullet.transform.position = firePoint.position;
        bullet.SetActive(true);
        float dir = transform.localScale.x > 0 ? 1 : -1;
        bullet.GetComponent<BulletScript>().SetDirection(dir);
    }

    private void ShootShotgun()
    {
        float baseDir = transform.localScale.x > 0 ? 1 : -1;
        float baseAngle = baseDir > 0 ? 0 : 180f;

        for (int i = 0; i < shotgunBulletCount; i++)
        {
            GameObject bullet = BulletPool.instance.GetBullet();
            if (bullet == null) continue;
            bullet.transform.position = firePoint.position;
            bullet.SetActive(true);

            float angleOffset = (i - (shotgunBulletCount - 1) / 2f) * (shotgunSpreadAngle / (shotgunBulletCount - 1));
            float finalAngle = baseAngle + angleOffset;
            Vector2 velocity = new Vector2(Mathf.Cos(finalAngle * Mathf.Deg2Rad), Mathf.Sin(finalAngle * Mathf.Deg2Rad)) * shotgunBulletSpeed;
            bullet.GetComponent<BulletScript>().SetVelocity(velocity);
        }
    }
}
