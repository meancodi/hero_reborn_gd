using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float startingHealth = 5f;
    [SerializeField] private float hurtFlashDuration = 0.1f;
    
    private float currentHealth;
    private bool dead = false;
    private SpriteRenderer sr;
    private Animator anim;
    private Renderer meshRenderer;

    public System.Action onDeath;

    private void Awake()
    {
        currentHealth = startingHealth;
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        meshRenderer = GetComponentInChildren<Renderer>();
    }

    public void SetStartingHealth(float h)
    {
        startingHealth = h;
        currentHealth = h;
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetStartingHealth() => startingHealth;


    public void TakeDamage(float damage)
    {
        if (dead) return;

        currentHealth -= damage;
        Debug.Log($"[EnemyHealth] {gameObject.name} took {damage} damage. HP: {currentHealth}");

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

    private IEnumerator HurtEffect()
    {
        SetVisualAlpha(0.4f);
        yield return new WaitForSeconds(hurtFlashDuration);
        SetVisualAlpha(1f);
    }

    private IEnumerator DeathEffect()
    {
        DisableEnemyLogic();
        onDeath?.Invoke();

        // 3 Fast blinks then GONE
        for (int i = 0; i < 3; i++)
        {
            SetVisualColor(Color.red);
            yield return new WaitForSeconds(0.05f);
            SetVisualColor(Color.white);
            yield return new WaitForSeconds(0.05f);
        }

        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void SetVisualAlpha(float alpha)
    {
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
        if (meshRenderer != null)
        {
            Color c = meshRenderer.material.color;
            c.a = alpha;
            meshRenderer.material.color = c;
        }
    }

    private void SetVisualColor(Color color)
    {
        if (sr != null) sr.color = color;
        if (meshRenderer != null) meshRenderer.material.color = color;
    }

    private void DisableEnemyLogic()
    {
        var patrol = GetComponent<GroundEnemyPatrol>();
        if (patrol != null) patrol.enabled = false;
        
        var attack = GetComponent<GroundEnemyAttack>();
        if (attack != null) attack.enabled = false;

        var rectBoss = GetComponent<RectangleFollow>();
        if (rectBoss != null) rectBoss.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }
}
