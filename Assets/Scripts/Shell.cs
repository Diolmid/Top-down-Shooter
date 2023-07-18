using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] private float forceMin, forceMax;

    private float _lifeTime = 4;
    private float _fadeTime = 2;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float force = Random.Range(forceMin, forceMax);
        _rb.AddForce(transform.right * force);
        _rb.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(_lifeTime);

        float percet = 0;
        float fadeSpeed = 1 / _fadeTime;
        var mat = GetComponent<Renderer>().material;
        var initialColour = mat.color;

        while (percet < 1)
        {
            percet += Time.time * fadeSpeed;
            mat.color = Color.Lerp(initialColour, Color.clear, percet);
            yield return null;
        }

        Destroy(gameObject);
    }
}
