using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float speed = 10f;
    [SerializeField] protected LayerMask collisionMask;

    private float _lifetime = 3;
    private float _distanceModifier = 0.1f;

    private void Start()
    {
        Destroy(gameObject, _lifetime);

        var initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if (initialCollisions.Length > 0 )
            OnHitObject(initialCollisions[0]);
    }

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

        if(Physics.Raycast(ray, out hit, moveDistance + _distanceModifier, collisionMask, QueryTriggerInteraction.Collide))
            OnHitObject(hit);
    }

    private void OnHitObject(RaycastHit hit)
    {
        var damageableObject = hit.collider.GetComponent<IDamageable>();
        if(damageableObject != null )
            damageableObject.TakeHit(damage, hit);

        Destroy(gameObject);
    }

    private void OnHitObject(Collider collider)
    {
        var damageableObject = collider.GetComponent<IDamageable>();
        if (damageableObject != null)
            damageableObject.TakeDamage(damage);

        Destroy(gameObject);
    }
}