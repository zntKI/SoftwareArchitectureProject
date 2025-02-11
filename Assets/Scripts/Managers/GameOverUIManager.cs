using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// UI Singleton manager for GameOverScreen, responsible for displaying the correct canvas on GameOver
/// </summary>
public class GameOverUIManager : MonoBehaviour
{
    public static GameOverUIManager Instance => instance;
    static GameOverUIManager instance;

    [SerializeField]
    private RectTransform gameOverWinPanel;
    [SerializeField]
    private RectTransform gameOverLosePanel;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new InvalidOperationException("There can only be only one UIManager in the scene!");
        }
    }

    void Start()
    {
        // Check to see the GameOver result and output the appropriate canvas
        if (GameManager.IsWaveOverWin)
        {
            gameOverWinPanel.gameObject.SetActive(true);
            gameOverLosePanel.gameObject.SetActive(false);
        }
        else
        {
            gameOverLosePanel.gameObject.SetActive(true);
            gameOverWinPanel.gameObject.SetActive(false);
        }
    }
}