//using UnityEngine;

//public class GroundEnemyAttack : MonoBehaviour
//{
//    [SerializeField] public float shootCooldown = 1f;
//    [SerializeField] public GameObject bulletPrefab;
//    [SerializeField] public Transform firePoint;

//    private Animator anim;
//    private GroundEnemyVisionBox vision;
//    private float cooldownTimer = 0f;

//    private void Awake()
//    {
//        vision = GetComponent<GroundEnemyVisionBox>();
//        anim = GetComponent<Animator>();
//    }

//    private void Update()
//    {
//        cooldownTimer += Time.deltaTime;

//        anim.SetBool("isDetected", vision.playerDetected);

//        if (!vision.playerDetected)
//            return;

//        if (cooldownTimer >= shootCooldown)
//        {
//            anim.SetTrigger("fire");
//            Shoot();
//            cooldownTimer = 0f;
//        }
//    }

//    private void Shoot()
//    {
//        GameObject bullet = BulletPool.instance.GetBullet();

//        bullet.transform.position = firePoint.position;

//        float dir = (transform.localScale.x > 0) ? 1 : -1;

//        bullet.SetActive(true);
//        bullet.GetComponent<BulletScript>().SetDirection(dir);
//    }

//}


using UnityEngine;

public class GroundEnemyAttack : MonoBehaviour
{
    [SerializeField] private float shootCooldown = 1f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private Animator anim;
    private GroundEnemyVisionBox vision;
    private float cooldownTimer;

    private void Awake()
    {
        vision = GetComponent<GroundEnemyVisionBox>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Drive animator
        anim.SetBool("isDetected", vision.playerDetected);

        if (!vision.playerDetected)
            return;

        if (cooldownTimer >= shootCooldown)
        {
            anim.SetTrigger("fire");
            Shoot();
            cooldownTimer = 0f;
        }
    }

    private void Shoot()
    {
        GameObject bullet = BulletPool.instance.GetBullet();
        bullet.transform.position = firePoint.position;

        float dir = transform.localScale.x > 0 ? 1 : -1;
        bullet.SetActive(true);
        bullet.GetComponent<BulletScript>().SetDirection(dir);
    }
}
