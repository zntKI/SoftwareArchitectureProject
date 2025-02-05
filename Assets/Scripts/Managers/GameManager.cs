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

    public static GameManager Instance => instance;
    static GameManager instance;

    public static bool IsGameOverWin => instance.isGameOverWin;
    private bool isGameOverWin;

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

        WaveManager.OnGameOver += GameOver;

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

    void GameOver(bool result)
    {
        isGameOverWin = result;

        // Switch to an end screen
        LoadNextScene();
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
        WaveManager.OnGameOver -= GameOver;

        GameUIManager.OnPauseMenuInteraction -= StopPlayTime;
    }
}