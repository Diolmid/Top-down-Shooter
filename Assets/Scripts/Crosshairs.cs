using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
    [SerializeField] private SpriteRenderer dot;
    [SerializeField] private Color originalDotColor;
    [SerializeField] private Color dotHighlightColor;
    [SerializeField] private LayerMask targetMask;

    private void Start()
    {
        Cursor.visible = false;
        originalDotColor = dot.color;
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward * -48 * Time.deltaTime);
    }

    public void DetectTargets(Ray ray)
    {
        if(Physics.Raycast(ray, 100, targetMask))
            dot.color = dotHighlightColor;
        else
            dot.color = originalDotColor;
    }
}
