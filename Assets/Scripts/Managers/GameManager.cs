using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => instance;
    static GameManager instance;

    public static event Action<int> OnMoneyUpdated;

    private int moneyAmount;

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

        EnemyController.OnMoneyDropped += UpdateMoney;
    }

    void UpdateMoney(int moneyToAdd)
    {
        instance.moneyAmount += moneyToAdd;
        OnMoneyUpdated?.Invoke(instance.moneyAmount);
    }

    void OnDestroy()
    {
        EnemyController.OnMoneyDropped -= UpdateMoney;
    }
}