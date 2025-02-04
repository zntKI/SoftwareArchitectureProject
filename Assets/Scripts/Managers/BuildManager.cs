using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance => instance;
    static BuildManager instance;

    public static event Action<int> OnMoneyUpdated;

    private int moneyAmount;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new InvalidOperationException("There can only be only one BuildManager in the scene!");
        }

        EnemyController.OnMoneyDropped += UpdateMoney;
    }

    void UpdateMoney(int moneyToAdd)
    {
        instance.moneyAmount += moneyToAdd;
        OnMoneyUpdated?.Invoke(instance.moneyAmount);
    }
}