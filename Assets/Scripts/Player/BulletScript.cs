using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float Maxlifetime = 10f;

    private float direction;
    private float lifetime;
    private bool hasHit = false;
    private bool hit = false;

    private BoxCollider2D box_collider;
    private Animator anim;

    private Vector2 velocity;
    private bool useVelocity = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        box_collider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        // Reset internal state when reused from pool
        hasHit = false;
        hit = false;
        lifetime = 0f;

        if (box_collider != null)
            box_collider.enabled = true;
    }

    private void Update()
    {
        if (hit) return;

        if (useVelocity)
        {
            transform.Translate(velocity * Time.deltaTime, Space.World);
        }
        else
        {
            float movementSpeed = speed * direction * Time.deltaTime;
            transform.Translate(Vector3.right * movementSpeed, Space.World);
        }

        lifetime += Time.deltaTime;
        if (lifetime >= Maxlifetime)
            Deactivate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        // Ignore collisions with the shooter
        if (CompareTag("PlayerBullet") && collision.CompareTag("Player")) return;
        if (CompareTag("EnemyBullet") && collision.CompareTag("Enemy")) return;

        // Ignore collisions with other bullets
        if (collision.CompareTag("PlayerBullet") || collision.CompareTag("EnemyBullet")) return;

        // Ignore generic triggers (like vision cones, detection zones) unless they are valid targets
        if (collision.isTrigger && !collision.CompareTag("Enemy") && !collision.CompareTag("Player")) return;

        hasHit = true;

        // ------------ PLAYER BULLET → damages ENEMY ------------
        if (CompareTag("PlayerBullet") && collision.CompareTag("Enemy"))
        {
            EnemyHealth eh = collision.GetComponent<EnemyHealth>();
            if (eh != null)
                eh.TakeDamage(1);

            Deactivate();
            return;
        }

        // ------------ ENEMY BULLET → damages PLAYER ------------
        if (CompareTag("EnemyBullet") && collision.CompareTag("Player"))
        {
            Health ph = collision.GetComponent<Health>();
            if (ph != null)
                ph.TakeDamage(1);

            Deactivate();
            return;
        }

        // ------------ Hits wall or something else ------------
        hit = true;
        if (box_collider != null)
            box_collider.enabled = false;

        if (anim != null)
            anim.SetTrigger("strike");
    }

    public void SetDirection(float _direction)
    {
        direction = Mathf.Sign(_direction);
        useVelocity = false;

        // Always reset collider
        if (box_collider != null)
            box_collider.enabled = true;

        // Reset rotation so bullets always face correct way
        transform.rotation = Quaternion.identity;

        // Flip sprite visually
        float scaleX = Mathf.Abs(transform.localScale.x) * direction;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
    }

    public void SetVelocity(Vector2 _velocity)
    {
        velocity = _velocity;
        useVelocity = true;
        
        if (box_collider != null)
            box_collider.enabled = true;

        // Face the direction of travel
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Visual scaling
        float scaleX = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void DisableImmediately()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        gameObject.SetActive(false);
    }
}
