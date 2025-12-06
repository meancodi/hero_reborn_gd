using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] bullets;
    [SerializeField] private AudioClip fireSound;
    private AudioSource audioSource;


    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private Rigidbody2D rb;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // grounded + not attacking + cooldown complete
        if (Input.GetMouseButtonDown(0) &&
            cooldownTimer >= attackCooldown &&
            playerMovement.canAttack() &&
            playerMovement.isGrounded())
        {
            Attack();
        }
    }

    private void Attack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0f;


        audioSource.PlayOneShot(fireSound);

        // Stop movement during attack
        playerMovement.isAttacking = true;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        int index = FindBullet();
        GameObject bullet = bullets[index];

        // Set position BEFORE activation 
        bullet.transform.position = firePoint.position;

        // Activate bullet THEN set direction
        bullet.SetActive(true);
        bullet.GetComponent<BulletScript>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private int FindBullet()
    {
        for (int i = 0; i < bullets.Length; i++)
            if (!bullets[i].activeInHierarchy)
                return i;

        return 0; // fallback
    }

    // called by animation event
    public void EndAttack()
    {
        playerMovement.isAttacking = false;
    }
}
