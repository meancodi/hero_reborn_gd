//using UnityEngine;

//public class Health : MonoBehaviour
//{
//    [SerializeField] private float startingHealth;

//    private Animator anim;

//    private float currentHealth;

//    private void Awake()
//    {
//        currentHealth = startingHealth;
//        anim = GetComponent<Animator>();
//    }

//    public void TakeDamage(float _damage)
//    {
//        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
//        print(currentHealth);

//        if(currentHealth > 0)
//        {
//            anim.SetTrigger("hurt");
//        }
//        else
//        {
//            anim.SetTrigger("die");
//        }

//    }

//}


using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth = 3f;
    [SerializeField] private float iFrameDuration = 1.0f;
    [SerializeField] private float flashInterval = 0.1f;

    private SpriteRenderer sr;
    private Animator anim;
    private float currentHealth;
    private bool isInvincible = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        currentHealth = startingHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
        print(currentHealth);
        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            StartCoroutine(Invulnerability());
        }
        else
        {
            anim.SetTrigger("die");
        }
    }

    private System.Collections.IEnumerator Invulnerability()
    {
        isInvincible = true;

        float elapsed = 0f;

        while (elapsed < iFrameDuration)
        {
            // Toggle visibility → blinking effect
            Color c = sr.color;
            c.a = (c.a == 1f ? 0.3f : 1f);
            sr.color = c;


            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        // End invulnerability
        sr.enabled = true;
        isInvincible = false;
    }
}
