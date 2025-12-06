//using UnityEngine;

//public class EnemyHealth : MonoBehaviour
//{
//    [SerializeField] private float startingHealth = 3f;

//    private float currentHealth;
//    private Animator anim;
//    private bool dead = false;

//    [SerializeField]private float invultime = 0.05f;

//    private float timeCounter = 0f;

//    private void Awake()
//    {
//        timeCounter = 0f;
//        currentHealth = startingHealth;
//        anim = GetComponent<Animator>();
//    }

//    private void Update()
//    {
//        if(timeCounter < invultime + 1.0f)
//            timeCounter += Time.deltaTime;
//    }

//    public void TakeDamage(float damage)
//    {
//        if (dead) return;

//        if (timeCounter < invultime) return;

//        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
//        timeCounter = 0f;

//        if (currentHealth > 0)
//        {
//            //anim.SetTrigger("hurt");
//            print("Took Damage");
//        }
//        else
//        {
//            dead = true;
//            //anim.SetTrigger("die");
//            print("Dead!");
//            OnDeath();
//        }
//    }

//    private void OnDeath()
//    {
//        // disable enemy AI, movement, attack
//        GetComponent<MonoBehaviour>().enabled = false;

//        // optionally destroy after animation finishes
//        Destroy(gameObject, 0.5f);
//    }
//}

using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float startingHealth = 3f;
    [SerializeField] private float hurtFlashDuration = 0.15f;
    [SerializeField] private int deathBlinkCount = 4;
    [SerializeField] private float deathBlinkInterval = 0.1f;

    private float currentHealth;
    private bool dead = false;

    private SpriteRenderer sr;
    private Animator anim;

    public System.Action onDeath;


    private void Awake()
    {
        currentHealth = startingHealth;
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (dead) return;

        currentHealth -= damage;

        if (currentHealth > 0)
        {
            StartCoroutine(HurtEffect());
        }
        else
        {
            dead = true;
            StartCoroutine(DeathEffect());
        }
    }

    // 🟡 Hurt = transparent flash
    private IEnumerator HurtEffect()
    {
        SetAlpha(0.4f);
        yield return new WaitForSeconds(hurtFlashDuration);
        SetAlpha(1f);
    }

    // 🔴 Death = red blinking
    private IEnumerator DeathEffect()
    {
        DisableEnemyLogic();

        // ✅ NOTIFY WAVE MANAGER
        onDeath?.Invoke();

        for (int i = 0; i < deathBlinkCount; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(deathBlinkInterval);
            sr.color = Color.white;
            yield return new WaitForSeconds(deathBlinkInterval);
        }

        Destroy(gameObject);
    }


    private void SetAlpha(float a)
    {
        Color c = sr.color;
        c.a = a;
        sr.color = c;
    }

    private void DisableEnemyLogic()
    {
        // Disable all enemy behaviour scripts here
        foreach (MonoBehaviour m in GetComponents<MonoBehaviour>())
        {
            if (m != this)
                m.enabled = false;
        }
    }
}
