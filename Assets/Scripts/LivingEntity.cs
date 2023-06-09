using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public event System.Action OnDeath;

    protected float health;
    protected bool dead;

    [SerializeField] protected float startingHealth;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public void TakeHit(float damage, RaycastHit hit)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
            Die();
    }

    protected void Die()
    {
        dead = true;
        if(OnDeath != null) 
            OnDeath();
        Destroy(gameObject);
    }
}