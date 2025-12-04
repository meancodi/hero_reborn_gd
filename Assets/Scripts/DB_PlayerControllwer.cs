using UnityEngine;

public class DB_PlayerControllwer : MonoBehaviour
{

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float airControlMultiplier = 0.7f;
    [SerializeField] private int maxJumps = 2;

    private int jumpCount;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxcollider;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxcollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        float horizontalInputAir = Input.GetAxis("Horizontal");
        float horizontalInputGround = Input.GetAxisRaw("Horizontal");



        if (isGrounded()) // on ground
        {
            anim.SetBool("run", horizontalInputGround != 0);
            body.linearVelocity = new Vector2(horizontalInputGround * moveSpeed, body.linearVelocity.y);
            jumpCount = 0;
        }
        else // in air
        {
            anim.SetBool("run", horizontalInputAir != 0);
            body.linearVelocity = new Vector2(horizontalInputAir * moveSpeed * airControlMultiplier, body.linearVelocity.y);
        }


        //Flip player
        if (horizontalInputAir > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInputAir < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);


        anim.SetBool("run", horizontalInputAir != 0);
        anim.SetBool("grounded", isGrounded());


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded() && (jumpCount < maxJumps))
        {
            Jump();
        }

    }

    private void Jump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpSpeed);
        anim.SetTrigger("jump");
        jumpCount++;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxcollider.bounds.center, boxcollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return true;
    }

}
