using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] private float flashTime;
    [SerializeField] private Sprite[] flashSprites;
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    [SerializeField] private GameObject flashHolder;

    private void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        flashHolder.SetActive(true);

        int flashSpriteIndex = Random.Range(0, flashSprites.Length);
        for (int i = 0; i < spriteRenderers.Length; i++)
            spriteRenderers[i].sprite = flashSprites[flashSpriteIndex];
        
        Invoke("Deactivate", flashTime);
    }

    private void Deactivate()
    {
        flashHolder.SetActive(false);
    }
}
