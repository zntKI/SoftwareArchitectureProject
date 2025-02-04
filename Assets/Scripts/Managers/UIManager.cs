using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance => instance;
    static UIManager instance;

    [SerializeField]
    private TextMeshProUGUI moneyAmountText;
    [SerializeField]
    private TextMeshProUGUI enemiesAmountText;

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
    }

    void UpdateMoneyText(int newMoneyAmount)
    {
        moneyAmountText.text = newMoneyAmount.ToString();
    }

    void UpdateEnemyAmountsText(int currAmount, int totalAmount)
    {
        enemiesAmountText.text = $"{currAmount}/{totalAmount}";
    }

    void OnDestroy()
    {
        BuildManager.OnMoneyUpdated -= UpdateMoneyText;
    }
}