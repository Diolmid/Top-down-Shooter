using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private int _activeMusicSourceIndex;

    private float _masterVolumePercent = .2f;
    private float _sfxVolumePercent = 1;
    private float _musicVolumePercent = 1;

    private AudioSource[] _musicSources;
    private Transform _audioListener;
    private Transform _player;

    private void Awake()
    {
        instance = this;

        _musicSources = new AudioSource[2];
        for(int i = 0; i < 2; i++)
        {
            var newMusicSource = new GameObject("Music source" + (i + 1));
            _musicSources[i] = newMusicSource.AddComponent<AudioSource>();
            newMusicSource.transform.parent = transform;
        }

        _audioListener = FindObjectOfType<AudioListener>().transform;
        _player = FindObjectOfType<Player>().transform;

    }

    private void Update()
    {
        if(_player != null)
            _audioListener.position = _player.position;
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        _activeMusicSourceIndex = 1 - _activeMusicSourceIndex;
        _musicSources[_activeMusicSourceIndex].clip = clip;
        _musicSources[_activeMusicSourceIndex].Play();

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        if (clip == null)
            return;

        AudioSource.PlayClipAtPoint(clip, position, _sfxVolumePercent * _masterVolumePercent);
    }

    private IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            _musicSources[_activeMusicSourceIndex].volume = Mathf.Lerp(0, _musicVolumePercent * _masterVolumePercent, percent);
            _musicSources[1 - _activeMusicSourceIndex].volume = Mathf.Lerp(_musicVolumePercent * _masterVolumePercent, 0, percent);
            yield return null;  
        }
    }
}
