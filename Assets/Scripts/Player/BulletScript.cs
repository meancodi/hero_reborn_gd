using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float Maxlifetime = 10.0f;
    private float direction;
    private bool hit;
    private float lifetime;

    private BoxCollider2D box_collider;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        box_collider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (hit) return;
        float movementSpeed = Time.deltaTime * speed * direction;
        transform.Translate(Vector3.right * movementSpeed, Space.World);
        lifetime += Time.deltaTime;
        if(lifetime > Maxlifetime) gameObject.SetActive(false);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        hit = true;
        box_collider.enabled = false;
        anim.SetTrigger("strike");

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        box_collider.enabled = false;
        anim.SetTrigger("strike");
    }

    public void SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        box_collider.enabled = true;
        transform.rotation = Quaternion.identity;

        float localScaleX = transform.localScale.x;
        if(Mathf.Sign(localScaleX) != _direction)
        {
            localScaleX = -localScaleX;
        }
        transform.localScale = new Vector3(localScaleX,transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate()
    {
        gameObject.SetActive (false);
    }

}
