using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static event Action OnPauseMenuInteraction;

    public static GameUIManager Instance => instance;
    static GameUIManager instance;

    [SerializeField]
    private TextMeshProUGUI moneyAmountText;
    [SerializeField]
    private TextMeshProUGUI enemiesAmountText;
    [SerializeField]
    private TextMeshProUGUI wavesAmountText;

    [SerializeField]
    private RectTransform pauseMenu;

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

        BuildManager.OnMoneyUpdated += UpdateMoneyText;

        WaveManager.OnUpdateEnemiesAmount += UpdateEnemyAmountsText;
        WaveManager.OnUpdateWavesAmount += UpdateWavesAmountText;
    }

    void Start()
    {
        pauseMenu.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OnPauseMenuInteraction?.Invoke();
            pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeInHierarchy);
        }
    }

    void UpdateMoneyText(int newMoneyAmount)
    {
        moneyAmountText.text = newMoneyAmount.ToString();
    }

    void UpdateEnemyAmountsText(int currAmount, int totalAmount)
    {
        enemiesAmountText.text = $"{currAmount}/{totalAmount}";
    }

    void UpdateWavesAmountText(int currAmount, int totalAmount)
    {
        wavesAmountText.text = $"{currAmount}/{totalAmount}";
    }

    void OnDestroy()
    {
        BuildManager.OnMoneyUpdated -= UpdateMoneyText;

        WaveManager.OnUpdateEnemiesAmount -= UpdateEnemyAmountsText;
        WaveManager.OnUpdateWavesAmount -= UpdateWavesAmountText;
    }
}