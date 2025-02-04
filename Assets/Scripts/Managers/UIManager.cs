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

        GameManager.OnMoneyUpdated += UpdateMoneyText;
    }

    private void UpdateMoneyText(int newMoneyAmount)
    {
        moneyAmountText.text = newMoneyAmount.ToString();
    }

    void OnDestroy()
    {
        GameManager.OnMoneyUpdated -= UpdateMoneyText;
    }
}