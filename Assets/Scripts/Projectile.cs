using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float speed = 10f;
    [SerializeField] protected LayerMask collisionMask;

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);

        transform.Translate(Vector3.forward * moveDistance);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
            OnHitObject(hit);
    }

    private void OnHitObject(RaycastHit hit)
    {
        var damageableObject = hit.collider.GetComponent<IDamageable>();
        if(damageableObject != null )
            damageableObject.TakeHit(damage, hit);

        Destroy(gameObject);
    }
}