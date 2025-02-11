using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// Main Singleton manager, responsible for switching between GameStates(Building and Spawning) and Scenes, and controlling the time scale
/// </summary>
public class GameManager : MonoBehaviour
{
    public static event Action OnBeginWave;
    public static event Action OnBeginBuilding;

    public static GameManager Instance => instance;
    static GameManager instance;

    /// <summary>
    /// Used by the GameOverUIManager to determine which canvas to display
    /// </summary>
    public static bool IsWaveOverWin => instance.isWaveOverWin;
    private bool isWaveOverWin;

    /// <summary>
    /// Used both by BuildManager and WaveManager to decide whether to start gameplay with Building or with Wave Spawning
    /// </summary>
    public static bool IsBuildFirst => instance.isBuildFirst;
    [SerializeField]
    private bool isBuildFirst = true;

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
        BuildManager.OnBuildingEnd += BuildingOver;

        GameUIManager.OnPauseMenuInteraction += StopPlayTime;
    }

    void Update()
    {
        // Input handling for time scale change
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale + 0.1f, 0.1f, 2f);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale - 0.1f, 0.1f, 2f);
        }
    }

    void LoadNextScene()
    {
        int sceneBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (sceneBuildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            sceneBuildIndex = 0;
        }

        SceneManager.LoadScene(sceneBuildIndex);
    }

    /// <summary>
    /// Called from UI button to load the StartMenuScene
    /// </summary>
    public void LoadFirstScene()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Called when PauseMenu gets Opened/Closed<br></br>
    /// Sets time scale temporarily
    /// </summary>
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

    /// <summary>
    /// Registers an Event from WaveManager when spawning enemies has ended
    /// </summary>
    /// <param name="result">Whether the wave was survived or not</param>
    /// <param name="isLastWave"></param>
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

    /// <summary>
    /// Registers an Event from BuildManager when building has ended
    /// </summary>
    void BuildingOver()
    {
        // Start wave spawning
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
        BuildManager.OnBuildingEnd -= BuildingOver;

        GameUIManager.OnPauseMenuInteraction -= StopPlayTime;
    }
}

/// <summary>
/// Used for indicating current game state<br></br>
/// Used by GameUIManager
/// </summary>
public enum GameState
{
    Building,
    Wave
}