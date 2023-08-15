using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel { Master, Sfx, Music};

    public static AudioManager instance;

    private int _activeMusicSourceIndex;

    private float _masterVolumePercent = .2f;
    private float _sfxVolumePercent = 1;
    private float _musicVolumePercent = 1;

    private SoundLibrary _library;
    private AudioSource[] _musicSources;
    private AudioSource _sfx2DSource;
    private Transform _audioListener;
    private Transform _player;

    private void Awake()
    {
        if(instance != null)
            Destroy(gameObject);
        else
            instance = this;

        DontDestroyOnLoad(gameObject);

        _musicSources = new AudioSource[2];
        for(int i = 0; i < 2; i++)
        {
            var newMusicSource = new GameObject("Music source" + (i + 1));
            _musicSources[i] = newMusicSource.AddComponent<AudioSource>();
            newMusicSource.transform.parent = transform;
        }

        var newSfx2DSource = new GameObject("2D sfx source");
        _sfx2DSource = newSfx2DSource.AddComponent<AudioSource>();
        newSfx2DSource.transform.parent = transform;

        _audioListener = FindObjectOfType<AudioListener>().transform;
        _player = FindObjectOfType<Player>().transform;
        _library = GetComponent<SoundLibrary>();

        _masterVolumePercent = PlayerPrefs.GetFloat("master volume", _masterVolumePercent);
        _sfxVolumePercent = PlayerPrefs.GetFloat("sfx volume", _sfxVolumePercent);
        _musicVolumePercent = PlayerPrefs.GetFloat("music volume", _musicVolumePercent);
    }

    private void Update()
    {
        if(_player != null)
            _audioListener.position = _player.position;
    }

    public void PlaySound2D(string soundName)
    {
        _sfx2DSource.PlayOneShot(_library.GetClipFromName(soundName), _sfxVolumePercent * _masterVolumePercent);
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                _masterVolumePercent = volumePercent; 
                break;
            case AudioChannel.Sfx:
                _sfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                _musicVolumePercent = volumePercent;
                break;
        }

        _musicSources[0].volume = _musicVolumePercent * _masterVolumePercent;
        _musicSources[1].volume = _musicVolumePercent * _masterVolumePercent;

        PlayerPrefs.SetFloat("master volume", _masterVolumePercent);
        PlayerPrefs.SetFloat("sfx volume", _sfxVolumePercent);
        PlayerPrefs.SetFloat("music volume", _musicVolumePercent);
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

    public void PlaySound(string soundName, Vector3 position)
    {
        PlaySound(_library.GetClipFromName(soundName), position);
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
