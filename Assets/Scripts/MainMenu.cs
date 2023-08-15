using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuHolder;
    [SerializeField] private GameObject optionsMenuHolder;

    [SerializeField] private int[] screenWidths;
    [SerializeField] private Slider[] volumeSliders;
    [SerializeField] private Toggle[] resolutionToggles;
    [SerializeField] private Toggle fullscreenToggle;

    private int _activeSreenResolutionIndex;

    private void Start()
    {
        _activeSreenResolutionIndex = PlayerPrefs.GetInt("sreen resolution index");
        bool isFullscreen = PlayerPrefs.GetInt("fullscreen") == 1;

        volumeSliders[0].value = AudioManager.instance.MasterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.MusicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.SfxVolumePercent;

        for (int i = 0; i < resolutionToggles.Length; i++)
            resolutionToggles[i].isOn = i == _activeSreenResolutionIndex;

        fullscreenToggle.isOn = isFullscreen;
    }

    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OptionsMenu()
    {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }

    public void BackToMainMenu()
    {
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
    }

    public void SetFullscreen(bool isFullsreen)
    {
        for (int i = 0; i < resolutionToggles.Length; i++)
            resolutionToggles[i].interactable = !isFullsreen;

        if (isFullsreen)
        {
            var allResolutions = Screen.resolutions;
            var maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, isFullsreen);
        }
        else
            SetScreenResolution(_activeSreenResolutionIndex);

        PlayerPrefs.SetInt("fullscreen", isFullsreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetScreenResolution(int i)
    {
        if (resolutionToggles[i].isOn)
        {
            _activeSreenResolutionIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);
            PlayerPrefs.SetInt("screen resolution index", _activeSreenResolutionIndex);
            PlayerPrefs.Save();
        }
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);

    }

    public void SetSfxVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);

    }
}
