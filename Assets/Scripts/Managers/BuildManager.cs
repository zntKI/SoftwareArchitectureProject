using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public enum BuildState
{
    Building,
    NotBuilding
}

[Serializable]
public struct BuyableTower
{
    public GameObject towerPrefab;
    public int price;
}

public class BuildManager : MonoBehaviour
{
    public static event Action OnBuilingEnd;

    public static event Action<int> OnMoneyUpdated;
    public static event Action<int> OnTimeUpdate;

    public static event Action<GameState> OnBuildingStartUI;
    /// <summary>
    /// <List with all the tower buy data, current amount of money>
    /// </summary>
    public static event Action<List<BuyableTower>, int> OnBuyingSetupUI;

    public static BuildManager Instance => instance;
    static BuildManager instance;

    private BuildState buildState;

    [SerializeField]
    private int buildTimeSec = 60;
    private float timeCounterForBuild;

    [SerializeField, Tooltip("Towers player can buy - EXACTLY 3")]
    private List<BuyableTower> buyables;

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

        GameManager.OnBeginBuilding += SetupBuilding;

        EnemyController.OnMoneyDropped += UpdateMoney;
    }

    void Start()
    {
        buildState = BuildState.NotBuilding;
        timeCounterForBuild = buildTimeSec;

        SetupBuilding();
    }

    void Update()
    {
        if (buildState == BuildState.Building)
        {
            timeCounterForBuild -= Time.deltaTime;
            OnTimeUpdate?.Invoke((int)Mathf.Ceil(timeCounterForBuild));

            if (timeCounterForBuild <= 0)
            {
                buildState = BuildState.NotBuilding;

                OnBuilingEnd?.Invoke();
            }
        }
    }

    void SetupBuilding()
    {
        buildState = BuildState.Building;
        timeCounterForBuild = buildTimeSec;

        // UI
        OnBuildingStartUI?.Invoke(GameState.Building);
        OnBuyingSetupUI?.Invoke(buyables, moneyAmount);
    }

    void UpdateMoney(int moneyToAdd)
    {
        instance.moneyAmount += moneyToAdd;
        OnMoneyUpdated?.Invoke(instance.moneyAmount);
    }

    void OnDestroy()
    {
        GameManager.OnBeginBuilding -= SetupBuilding;

        EnemyController.OnMoneyDropped -= UpdateMoney;
    }
}