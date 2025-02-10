using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public enum BuildState
{
    Building,
    Buying,
    UpgradingSelling,
    NotBuilding
}

public class BuyableTower
{
    public GameObject towerPrefab;
    public int price;

    public BuyableTower(GameObject towerPrefab, int price)
    {
        this.towerPrefab = towerPrefab;
        this.price = price;
    }
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

    /// <summary>
    /// Called either when a player deselects a tower, or when he selects the same tower he deselected the last time<br></br>
    /// which shows/hides the Upgrade/Sell UI panel
    /// </summary>
    public static event Action OnUpgradePanelModifyVisibility;
    /// <summary>
    /// To update the Upgrade panel UI with the newly selected tower information, with current amount of money
    /// </summary>
    public static event Action<TowerController, int> OnUpgradingUpdateUI;

    public static BuildManager Instance => instance;
    static BuildManager instance;

    private BuildState buildState;

    [SerializeField]
    private int buildTimeSec = 60;
    private float timeCounterForBuild;

    [SerializeField]
    private int moneyAmount;


    // BUYING

    [SerializeField, Tooltip("Towers player can buy - EXACTLY 3")]
    private GameObject[] buyableTowers;

    /// <summary>
    /// Contains information about the buyable tower with its price for ease of use instead of having
    /// to retrieve the price constantly from the Model
    /// </summary>
    private List<BuyableTower> buyables = new List<BuyableTower>();
    private BuyableTower currentBuySelectedTower;

    [SerializeField]
    private GameObject buildSpotPrefab;
    private List<GameObject> buildSpots = new List<GameObject>();


    // UPGRADING/SELLING

    private TowerController currentUpgradeSelectedTower;

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

        TowerController.OnTowerClicked += CheckForUpgrading;
    }

    void Start()
    {
        foreach (var tower in buyableTowers)
        {
            buyables.Add(new BuyableTower(tower, tower.GetComponent<TowerModel>().BuyPrice));
        }

        buildSpots = GameObject.FindGameObjectsWithTag("BuildSpot").ToList();
        ModifyBuildSpotsVisibility(false);

        buildState = BuildState.NotBuilding;
        timeCounterForBuild = buildTimeSec;

        OnMoneyUpdated?.Invoke(moneyAmount);

        SetupBuilding();
    }

    void SetupBuilding()
    {
        buildState = BuildState.Buying;
        timeCounterForBuild = buildTimeSec;

        // UI
        OnBuildingStartUI?.Invoke(GameState.Building);
        OnBuyingUpdateUI?.Invoke(buyables, moneyAmount);
    }

    void SwitchBuildState(BuildState newBuildState)
    {
        if (buildState == BuildState.Buying &&
            newBuildState == BuildState.UpgradingSelling) // Stop buying
        {
            currentBuySelectedTower = null;
            ModifyBuildSpotsVisibility(false);
            OnBuyingUpdateUI?.Invoke(buyables, moneyAmount);
        }
        else if (buildState == BuildState.UpgradingSelling &&
            newBuildState == BuildState.Buying) // Stop upgrading/selling
        {
            if (currentUpgradeSelectedTower != null)
            {
                currentUpgradeSelectedTower.ModifyOutlineVisibility();
                OnUpgradePanelModifyVisibility?.Invoke();
                currentUpgradeSelectedTower = null;
            }
        }

        buildState = newBuildState;
    }

    void Update()
    {
        if (buildState != BuildState.NotBuilding)
        {
            timeCounterForBuild -= Time.deltaTime;
            OnTimeUpdate?.Invoke((int)Mathf.Ceil(timeCounterForBuild));

            if (timeCounterForBuild <= 0)
            {
                buildState = BuildState.NotBuilding;

                ResetVars();

                OnBuilingEnd?.Invoke();
            }
        }
    }

    void ResetVars()
    {
        currentBuySelectedTower = null;

        if (currentUpgradeSelectedTower != null)
            currentUpgradeSelectedTower.ModifyOutlineVisibility();
        currentUpgradeSelectedTower = null;
        ModifyBuildSpotsVisibility(false);
    }

    void UpdateMoney(int moneyToAdd)
    {
        moneyAmount += moneyToAdd;
        OnMoneyUpdated?.Invoke(moneyAmount);
    }

    void SetSelectedTower(GameObject selectedTower)
    {
        if (buildState != BuildState.Buying)
        {
            SwitchBuildState(BuildState.Buying);
        }

        if (selectedTower != null)
        {
            ModifyBuildSpotsVisibility(false); // Just in case another affordable tower had been selected beforehand

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

            SwitchBuildState(BuildState.Building);
        }
        else
        {
            Debug.LogWarning("Not enough money to buy the tower!");
        }
    }

    void CheckForUpgrading(TowerController tower)
    {
        if (buildState != BuildState.UpgradingSelling)
        {
            SwitchBuildState(BuildState.UpgradingSelling);
        }

        if (currentUpgradeSelectedTower == null) // If first ever tower selected
        {
            currentUpgradeSelectedTower = tower;

            currentUpgradeSelectedTower.ModifyOutlineVisibility(); // Make outline visible
            OnUpgradingUpdateUI?.Invoke(currentUpgradeSelectedTower, moneyAmount); // Update Upgrade panel
        }
        else if (tower == currentUpgradeSelectedTower) // If previously selected tower now gets deselected
        {
            currentUpgradeSelectedTower.ModifyOutlineVisibility(); // Disable outline
            OnUpgradePanelModifyVisibility?.Invoke(); // Hide Upgrade panel

            currentUpgradeSelectedTower = null;
        }
        else // If switching between towers to upgrade
        {
            currentUpgradeSelectedTower.ModifyOutlineVisibility(); // Disable outline for previously selected tower

            currentUpgradeSelectedTower = tower;

            currentUpgradeSelectedTower.ModifyOutlineVisibility(); // Enable outline for newly selected tower
            OnUpgradingUpdateUI?.Invoke(currentUpgradeSelectedTower, moneyAmount); // Update Upgrade panel
        }
    }

    public void OnUpgradeBtnClicked()
    {
        int price = currentUpgradeSelectedTower.Upgrade();
        moneyAmount -= price;
        OnMoneyUpdated.Invoke(moneyAmount);

        OnUpgradingUpdateUI?.Invoke(currentUpgradeSelectedTower, moneyAmount); // Update Upgrade panel

        OnBuyingUpdateUI?.Invoke(buyables, moneyAmount);
    }

    public void OnSellBtnClicked()
    {
        int price = currentUpgradeSelectedTower.Sell();
        moneyAmount += price;
        OnMoneyUpdated.Invoke(moneyAmount);

        OnUpgradePanelModifyVisibility?.Invoke();

        // Delete the sold tower
        Destroy(currentUpgradeSelectedTower.gameObject);
        // Reinsert the deleted buildspot when the tower was initially built
        GameObject newBuildSpot = Instantiate(buildSpotPrefab, currentUpgradeSelectedTower.transform.position, Quaternion.identity);
        newBuildSpot.SetActive(false);
        buildSpots.Add(newBuildSpot);

        OnBuyingUpdateUI?.Invoke(buyables, moneyAmount);

        SwitchBuildState(BuildState.Building);
    }

    void OnDestroy()
    {
        GameManager.OnBeginBuilding -= SetupBuilding;

        EnemyController.OnMoneyDropped -= UpdateMoney;

        GameUIManager.OnTowerSelection -= SetSelectedTower;

        BuildSpotController.OnBuildSpotClicked -= PlaceTower;

        TowerController.OnTowerClicked -= CheckForUpgrading;
    }
}