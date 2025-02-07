using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static event Action OnBeginWave;
    public static event Action OnBeginBuilding;

    public static GameManager Instance => instance;
    static GameManager instance;

    public static bool IsWaveOverWin => instance.isWaveOverWin;
    private bool isWaveOverWin;

    private float gameTimeScale = 1f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return; // just to be sure
        }

        WaveManager.OnWaveOver += WaveOver;
        BuildManager.OnBuilingEnd += BuildingOver;

        GameUIManager.OnPauseMenuInteraction += StopPlayTime;
    }

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale + 0.1f, 0.1f, 1f);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale - 0.1f, 0.1f, 1f);
        }
    }

    public void LoadNextScene()
    {
        int sceneBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (sceneBuildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            sceneBuildIndex = 0;
        }

        SceneManager.LoadScene(sceneBuildIndex);
    }

    public void LoadFirstScene()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
    }

    void StopPlayTime()
    {
        if (Time.timeScale != 0)
        {
            gameTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = gameTimeScale;
        }
    }

    void WaveOver(bool result, bool isLastWave)
    {
        isWaveOverWin = result;

        if ((isWaveOverWin && isLastWave) || !isWaveOverWin) // If win but last wave or lose in any wave
            LoadNextScene();
        else if (isWaveOverWin)
        {
            // Start build sess
            OnBeginBuilding?.Invoke();
        }
    }

    void BuildingOver()
    {
        OnBeginWave?.Invoke();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnDestroy()
    {
        WaveManager.OnWaveOver -= WaveOver;
        BuildManager.OnBuilingEnd -= BuildingOver;

        GameUIManager.OnPauseMenuInteraction -= StopPlayTime;
    }
}

public enum GameState
{
    Building,
    Wave
}