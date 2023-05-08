using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    protected float health;
    protected bool dead;

    [SerializeField] protected float startingHealth;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public void TakeHit(float damage, RaycastHit hit)
    {
        health -= damage;

        if(health <= 0 && !dead)
        {
            Die();
        }
    }

    protected void Die()
    {
        dead = true;
        Destroy(gameObject);
    }
}