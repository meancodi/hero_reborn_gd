//using UnityEngine;

//public class Health : MonoBehaviour
//{
//    [SerializeField] private float startingHealth = 3f;
//    [SerializeField] private float iFrameDuration = 1.0f;
//    [SerializeField] private float flashInterval = 0.1f;

//    private SpriteRenderer sr;
//    private Animator anim;
//    private float currentHealth;
//    private bool isInvincible = false;

//    private void Awake()
//    {
//        sr = GetComponent<SpriteRenderer>();
//        anim = GetComponent<Animator>();
//        currentHealth = startingHealth;
//    }

//    public void TakeDamage(float damage)
//    {
//        if (isInvincible) return;

//        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
//        print(currentHealth);
//        if (currentHealth > 0)
//        {
//            anim.SetTrigger("hurt");
//            StartCoroutine(Invulnerability());
//        }
//        else
//        {
//            anim.SetTrigger("die");
//        }
//    }

//    private System.Collections.IEnumerator Invulnerability()
//    {
//        isInvincible = true;

//        float elapsed = 0f;

//        while (elapsed < iFrameDuration)
//        {
//            // Toggle visibility → blinking effect
//            Color c = sr.color;
//            c.a = (c.a == 1f ? 0.3f : 1f);
//            sr.color = c;


//            yield return new WaitForSeconds(flashInterval);
//            elapsed += flashInterval;
//        }

//        // End invulnerability
//        sr.enabled = true;
//        isInvincible = false;
//    }
//}


using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth = 3f;
    [SerializeField] private float iFrameDuration = 1.0f;
    [SerializeField] private float flashInterval = 0.1f;


    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip dieVoice;



    private AudioSource audioSource;


    private SpriteRenderer sr;
    private Animator anim;
    private float currentHealth;

    private bool isInvincible = false;
    private bool isDead = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        currentHealth = startingHealth;
        audioSource = GetComponent<AudioSource>();

    }

    public void TakeDamage(float damage)
    {
        if (isInvincible || isDead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("HP: " + currentHealth);

        if (currentHealth > 0)
        {
            sfxSource.PlayOneShot(hurtSound);
            anim.SetTrigger("hurt");
            StartCoroutine(Invulnerability());
        }

        else
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        isInvincible = true; // hard lock all further damage
        sfxSource.PlayOneShot(dieVoice);

        StopAllCoroutines();

        // Turn player red
        sr.color = Color.red;
        sr.enabled = true;

        anim.SetTrigger("die");

        // Notify GameManager (NO delay logic here)
        GameManager.instance.PlayerDefeated();
    }

    private IEnumerator Invulnerability()
    {
        isInvincible = true;

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < iFrameDuration && !isDead)
        {
            visible = !visible;
            sr.color = new Color(
                sr.color.r,
                sr.color.g,
                sr.color.b,
                visible ? 1f : 0.3f
            );

            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        // Reset visuals
        sr.color = Color.white;
        isInvincible = false;
    }
}
