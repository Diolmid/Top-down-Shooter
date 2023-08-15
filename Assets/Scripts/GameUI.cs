using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Image fadePlane;
    [SerializeField] private GameObject gameOverUI;

    [SerializeField] private RectTransform newWaveBanner;
    [SerializeField] private TextMeshProUGUI newWaveTitle;
    [SerializeField] private TextMeshProUGUI newWaveEnemyCount;

    private Spawner _spawner;

    private void Awake()
    {
        _spawner = FindAnyObjectByType<Spawner>();
        _spawner.OnNewWave += OnNewWave;
    }

    private void Start()
    {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
    }

    private void OnNewWave(int waveNumber)
    {
        string enemyCountString = (_spawner.waves[waveNumber - 1].infinite) ? "Infinite" : _spawner.waves[waveNumber - 1].enemyCount + "";
        string[] numbers = { "One", "Two", "Three", "Four", "Five" };

        newWaveTitle.text = $"- Wave {numbers[waveNumber - 1]} - ";
        newWaveEnemyCount.text = $"Enemies: {enemyCountString}";

        StopCoroutine(AnimateNewWaveBanner());
        StartCoroutine(AnimateNewWaveBanner());
    }

    private IEnumerator AnimateNewWaveBanner()
    {
        int direction = 1;
        float animationPercent = 0;
        float speed = 3;
        float delayTime = 1.5f;

        float endDelayTime = Time.time + 1 / speed + delayTime;

        while(animationPercent >= 0)
        {
            animationPercent += Time.deltaTime * speed * direction;

            if(animationPercent >= 1)
            {
                animationPercent = 1;
                if(Time.time > endDelayTime)
                    direction = -1;
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-65, 65, animationPercent);
            yield return null;
        }
    }

    private void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        gameOverUI.SetActive(true);

    }

    private IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    // UI Input

    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }
}
