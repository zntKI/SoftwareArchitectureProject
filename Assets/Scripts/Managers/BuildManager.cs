using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public enum BuildState
{
    Building,
    NotBuilding
}

[Serializable]
public class BuyableTower
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
    public static event Action<List<BuyableTower>, int> OnBuyingUpdateUI;

    public static BuildManager Instance => instance;
    static BuildManager instance;

    private BuildState buildState;

    [SerializeField]
    private int buildTimeSec = 60;
    private float timeCounterForBuild;

    [SerializeField]
    private int moneyAmount;

    [SerializeField, Tooltip("Towers player can buy - EXACTLY 3")]
    private List<BuyableTower> buyables;
    private BuyableTower currentBuySelectedTower;
    private List<GameObject> buildSpots = new List<GameObject>();

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

        GameUIManager.OnTowerSelection += SetSelectedTower;

        BuildSpotController.OnBuildSpotClicked += PlaceTower;
    }

    void Start()
    {
        buildSpots = GameObject.FindGameObjectsWithTag("BuildSpot").ToList();
        ModifyBuildSpotsVisibility(false);

        buildState = BuildState.NotBuilding;
        timeCounterForBuild = buildTimeSec;

        OnMoneyUpdated?.Invoke(moneyAmount);

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

                ModifyBuildSpotsVisibility(false);
                OnBuilingEnd?.Invoke();
            }
        }
    }

    void UpdateMoney(int moneyToAdd)
    {
        moneyAmount += moneyToAdd;
        OnMoneyUpdated?.Invoke(moneyAmount);
    }

    void SetupBuilding()
    {
        buildState = BuildState.Building;
        timeCounterForBuild = buildTimeSec;

        // UI
        OnBuildingStartUI?.Invoke(GameState.Building);
        OnBuyingUpdateUI?.Invoke(buyables, moneyAmount);
    }

    void SetSelectedTower(GameObject selectedTower)
    {
        if (selectedTower != null)
        {
            currentBuySelectedTower = buyables.FirstOrDefault(b => b.towerPrefab == selectedTower);
            if (currentBuySelectedTower == null)
            {
                Debug.LogError("No such tower available for buying!");
            }
            else if (moneyAmount >= currentBuySelectedTower.price) // When a valid affordable tower is selected
            {
                // Turn on all possible spots to place the towers
                ModifyBuildSpotsVisibility(true);
            }
        }
        else
        {
            currentBuySelectedTower = null;
            ModifyBuildSpotsVisibility(false);
        }
    }

    void ModifyBuildSpotsVisibility(bool shouldMakeVisible)
    {
        foreach (var spot in buildSpots)
        {
            spot.SetActive(shouldMakeVisible);
        }
    }

    void PlaceTower(Transform buildSpotTransform)
    {
        if (currentBuySelectedTower == null)
        {
            Debug.LogError("No buy tower selected in order to place on map!");
            return;
        }

        if (moneyAmount >= currentBuySelectedTower.price)
        {
            // Build tower on same position as build spot
            Instantiate(currentBuySelectedTower.towerPrefab, buildSpotTransform.position, Quaternion.identity);

            // Update collection after build spot being destroyed
            buildSpots.Remove(buildSpotTransform.gameObject);
            // Disable again
            ModifyBuildSpotsVisibility(false);

            // Reduce money by cost of tower
            moneyAmount -= currentBuySelectedTower.price;
            // No current selected tower for buying
            currentBuySelectedTower = null;

            // Update UI
            OnMoneyUpdated?.Invoke(moneyAmount);
            OnBuyingUpdateUI?.Invoke(buyables, moneyAmount);
        }
        else
        {
            Debug.LogWarning("Not enough money to buy the tower!");
        }
    }

    void OnDestroy()
    {
        GameManager.OnBeginBuilding -= SetupBuilding;

        EnemyController.OnMoneyDropped -= UpdateMoney;

        GameUIManager.OnTowerSelection -= SetSelectedTower;

        BuildSpotController.OnBuildSpotClicked -= PlaceTower;
    }
}