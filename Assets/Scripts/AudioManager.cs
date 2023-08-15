using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel { Master, Sfx, Music};

    public static AudioManager instance;

    private int _activeMusicSourceIndex;

    public float MasterVolumePercent { get; private set; } = .2f;
    public float SfxVolumePercent { get; private set; } = 1;
    public float MusicVolumePercent { get; private set; } = 1;

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
        {
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
            _player = FindObjectOfType<Player>()?.transform;
            _library = GetComponent<SoundLibrary>();

            MasterVolumePercent = PlayerPrefs.GetFloat("master volume", MasterVolumePercent);
            SfxVolumePercent = PlayerPrefs.GetFloat("sfx volume", SfxVolumePercent);
            MusicVolumePercent = PlayerPrefs.GetFloat("music volume", MusicVolumePercent);
        }
    }

    private void Update()
    {
        if(_player != null)
            _audioListener.position = _player.position;
    }

    public void PlaySound2D(string soundName)
    {
        _sfx2DSource.PlayOneShot(_library.GetClipFromName(soundName), SfxVolumePercent * MasterVolumePercent);
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                MasterVolumePercent = volumePercent; 
                break;
            case AudioChannel.Sfx:
                SfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                MusicVolumePercent = volumePercent;
                break;
        }

        _musicSources[0].volume = MusicVolumePercent * MasterVolumePercent;
        _musicSources[1].volume = MusicVolumePercent * MasterVolumePercent;

        PlayerPrefs.SetFloat("master volume", MasterVolumePercent);
        PlayerPrefs.SetFloat("sfx volume", SfxVolumePercent);
        PlayerPrefs.SetFloat("music volume", MusicVolumePercent);
        PlayerPrefs.Save();
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

        AudioSource.PlayClipAtPoint(clip, position, SfxVolumePercent * MasterVolumePercent);
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
            _musicSources[_activeMusicSourceIndex].volume = Mathf.Lerp(0, MusicVolumePercent * MasterVolumePercent, percent);
            _musicSources[1 - _activeMusicSourceIndex].volume = Mathf.Lerp(MusicVolumePercent * MasterVolumePercent, 0, percent);
            yield return null;  
        }
    }
}
