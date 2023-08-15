using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip mainTheme;
    [SerializeField] private AudioClip menuTheme;

    private void Start()
    {
        AudioManager.instance.PlayMusic(menuTheme, 2);
    }

    private void Update()
    {
        
    }
}
