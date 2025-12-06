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

        float movementSpeed = speed * direction * Time.deltaTime;
        transform.Translate(Vector3.right * movementSpeed, Space.World);

        lifetime += Time.deltaTime;
        if (lifetime >= Maxlifetime)
            Deactivate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;
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

        // Always reset collider
        if (box_collider != null)
            box_collider.enabled = true;

        // Reset rotation so bullets always face correct way
        transform.rotation = Quaternion.identity;

        // Flip sprite visually
        float scaleX = Mathf.Abs(transform.localScale.x) * direction;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate()
    {
        // Disable instantly (object pool safe)
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
