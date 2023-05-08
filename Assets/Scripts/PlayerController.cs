using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Vector3 _velocity;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
    }

    public void Move(Vector3 _velocity)
    {
        this._velocity = _velocity;
    }

    public void LookAt(Vector3 lookPoint)
    {
        Vector3 normalizeLookPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(normalizeLookPoint);
    }
}