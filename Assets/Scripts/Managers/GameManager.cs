using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static event Action OnBeginWave;

    public static GameManager Instance => instance;
    static GameManager instance;

    public static bool IsGameOverWin => instance.isGameOverWin;
    private bool isGameOverWin;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new InvalidOperationException("There can only be only one GameManager in the scene!");
        }

        WaveManager.OnGameOver += GameOver;
    }

    void Start()
    {
        OnBeginWave?.Invoke();
    }

    void GameOver(bool result)
    {
        isGameOverWin = result;

        // Switch to an end screen
        // There, ask for the GameManager.IsGameOverWin
    }

    void OnDestroy()
    {
        WaveManager.OnGameOver -= GameOver;
    }
}