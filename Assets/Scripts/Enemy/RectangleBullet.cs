using UnityEngine;

public class RectangleBullet : MonoBehaviour
{
    private Vector2 velocity;
    private float lifetime;
    private float maxRange = 10f; // Range limit
    private float speed;

    public void Initialize(Vector2 dir, float bulletSpeed, float range)
    {
        speed = bulletSpeed;
        velocity = dir * speed;
        maxRange = range;
        lifetime = maxRange / speed; // Time it takes to reach max range

        // Create visual: Small Circle
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visual.transform.SetParent(transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); // Small circle
        
        // Remove sphere collider, use 2D trigger instead
        Destroy(visual.GetComponent<SphereCollider>());
        visual.GetComponent<Renderer>().material.color = Color.yellow;

        // Add 2D Trigger
        CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.15f;

        // Rigidbody for movement
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = velocity;
        
        // Auto-destroy after range reached
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Ignore the boss itself and other bullets
        if (collision.CompareTag("Enemy") || collision.name.Contains("ShotgunBullet"))
        {
            return;
        }

        // 2. Damage player if we hit them
        if (collision.CompareTag("Player"))
        {
            Health ph = collision.GetComponent<Health>();
            if (ph != null) ph.TakeDamage(1);
            Destroy(gameObject);
        }
        // 3. Hit ground or walls
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
