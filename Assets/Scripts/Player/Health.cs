using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;

    private Animator anim;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        print(currentHealth);
        
        if(currentHealth > 0)
        {
            anim.SetTrigger("hurt");
        }
        else
        {
            anim.SetTrigger("die");
        }

    }

}
