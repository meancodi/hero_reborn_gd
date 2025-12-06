//using UnityEngine;

//public class EnemyDamageReceiver : MonoBehaviour
//{
//    [SerializeField] private float damage = 1f;

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        // If hit by a player bullet, deal damage
//        if (collision.CompareTag("PlayerBullet"))
//        {
//            collision.GetComponent<BulletScript>().SetDirection(0); // optional stop bullet
//            GetComponent<EnemyHealth>().TakeDamage(damage);
//        }
//    }
//}


using UnityEngine;

public class EnemyDamageReceiver : MonoBehaviour
{
    [SerializeField] private float damage = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerBullet"))
            return;

        EnemyHealth health = GetComponent<EnemyHealth>();
        if (health == null)
            return;

        health.TakeDamage(damage);

        // HARD STOP the bullet
        BulletScript bullet = collision.GetComponent<BulletScript>();
        if (bullet != null)
            bullet.DisableImmediately();
    }
}
