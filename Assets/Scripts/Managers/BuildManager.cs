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

/// <summary>
/// Container class containing information about the Prefab of a buyable tower as well its price
/// </summary>
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

/// <summary>
/// Building Singleton manager, responsible for tower buying, upgrading and selling as well as communicating with other managers
/// </summary>
public class BuildManager : MonoBehaviour
{
    public static event Action OnBuildingEnd;

    public static event Action<int> OnMoneyUpdated;
    public static event Action<int> OnTimeUpdate;

    public static event Action<GameState> OnBuildingStartUI;
    /// <summary>
    /// <List with all the tower buy data, current amount of money>
    /// </summary>
    public static event Action<List<BuyableTower>, int> OnBuyingUpdateUI;

    /// <summary>
    /// Shows/hides the Upgrade/Sell UI panel
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
    private int buildTimeSec = 20;
    [SerializeField]
    private int buildTimeAddAfterEachWave = 10;
    private float timeCounterForBuild;

    [SerializeField]
    private int moneyAmount;


    // BUYING

    [SerializeField, Tooltip("Towers player can buy - EXACTLY 3")]
    private GameObject[] buyableTowers;

    /// <summary>
    /// Contains information about the buyable tower with its price for ease of use instead of having
    /// to retrieve the price constantly from the TowerModel
    /// </summary>
    private List<BuyableTower> buyables = new List<BuyableTower>();
    private BuyableTower currentBuySelectedTower;

    /// <summary>
    /// A game object that is used for marking possible location of a new tower to be build
    /// </summary>
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
        // Retrieve information only once - from the start
        foreach (var tower in buyableTowers)
        {
            buyables.Add(new BuyableTower(tower, tower.GetComponent<TowerModel>().BuyPrice));
        }

        buildSpots = GameObject.FindGameObjectsWithTag("BuildSpot").ToList();
        ModifyBuildSpotsVisibility(false);

        OnMoneyUpdated?.Invoke(moneyAmount); // Update Money UI text

        buildState = BuildState.NotBuilding;
        buildTimeSec -= buildTimeAddAfterEachWave; // So that the first time, it increments it and gets it back to initial value

        if (GameManager.IsBuildFirst)
        {
            SetupBuilding();
        }
    }

    void SetupBuilding()
    {
        buildState = BuildState.Buying;
        buildTimeSec += buildTimeAddAfterEachWave; // Each following building sess, increase the build time
        timeCounterForBuild = buildTimeSec;

        // UI
        OnBuildingStartUI?.Invoke(GameState.Building); // Hide Wave UI and show Build UI
        OnBuyingUpdateUI?.Invoke(buyables, moneyAmount); // Update Buy UI panel
    }

    /// <summary>
    /// Used for resetting variables that belong to previous state
    /// </summary>
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
        if (buildState != BuildState.NotBuilding) // If in any kind of building, pass build time
        {
            timeCounterForBuild -= Time.deltaTime;
            OnTimeUpdate?.Invoke((int)Mathf.Ceil(timeCounterForBuild));

            if (timeCounterForBuild <= 0) // On build time over, switch game states
            {
                buildState = BuildState.NotBuilding;

                ResetVars();

                OnBuildingEnd?.Invoke();
            }
        }
    }

    /// <summary>
    /// Called when Building finished to reset variables
    /// </summary>
    void ResetVars()
    {
        currentBuySelectedTower = null;

        if (currentUpgradeSelectedTower != null)
            currentUpgradeSelectedTower.ModifyOutlineVisibility();
        currentUpgradeSelectedTower = null;
        ModifyBuildSpotsVisibility(false);
    }

    /// <summary>
    /// Called when money amount changes in order to update it and its UI element
    /// </summary>
    void UpdateMoney(int moneyToAdd)
    {
        moneyAmount += moneyToAdd;
        OnMoneyUpdated?.Invoke(moneyAmount);
    }

    /// <summary>
    /// Called when a tower from the Buy UI panel has been selected
    /// </summary>
    void SetSelectedTower(GameObject selectedTower)
    {
        if (buildState != BuildState.Buying) // Stop tower upgrading/selling process if present
        {
            SwitchBuildState(BuildState.Buying);
        }

        if (selectedTower != null) // If tower gets selected
        {
            ModifyBuildSpotsVisibility(false); // Just in case another affordable tower had been selected beforehand

            currentBuySelectedTower = buyables.FirstOrDefault(b => b.towerPrefab == selectedTower);
            if (currentBuySelectedTower == null) // Tower buying collections in BuildManager and GameUIManager do not match
            {
                Debug.LogError("No such tower available for buying!");
            }
            else if (moneyAmount >= currentBuySelectedTower.price) // When a valid affordable tower is selected
            {
                // Turn on all possible spots to place the towers
                ModifyBuildSpotsVisibility(true);
            }
        }
        else // If tower is deselected
        {
            currentBuySelectedTower = null;
            ModifyBuildSpotsVisibility(false);
        }
    }

    /// <summary>
    /// Hide/show build spots game objects
    /// </summary>
    /// <param name="shouldMakeVisible"></param>
    void ModifyBuildSpotsVisibility(bool shouldMakeVisible)
    {
        foreach (var spot in buildSpots)
        {
            spot.SetActive(shouldMakeVisible);
        }
    }

    /// <summary>
    /// Called when one of the build spots has been clicked
    /// </summary>
    void PlaceTower(Transform buildSpotTransform)
    {
        if (currentBuySelectedTower == null)
        {
            Debug.LogError("No buy tower selected in order to place on map!");
            return;
        }

        if (moneyAmount >= currentBuySelectedTower.price) // If enough money, build tower
        {
            // Build tower on same position as build spot
            Instantiate(currentBuySelectedTower.towerPrefab, buildSpotTransform.position, Quaternion.identity).GetComponent<TowerController>().Init();

            // Update collection after build spot being destroyed
            buildSpots.Remove(buildSpotTransform.gameObject);
            // Disable again
            ModifyBuildSpotsVisibility(false);

            // Reduce money by cost of tower
            UpdateMoney(-currentBuySelectedTower.price);
            // No current selected tower for buying
            currentBuySelectedTower = null;

            // Update UI
            OnBuyingUpdateUI?.Invoke(buyables, moneyAmount);

            SwitchBuildState(BuildState.Building);
        }
        else
        {
            Debug.LogWarning("Not enough money to buy the tower!");
        }
    }

    /// <summary>
    /// Called when in-game tower object has been clicked
    /// </summary>
    void CheckForUpgrading(TowerController tower)
    {
        if (buildState != BuildState.UpgradingSelling) // Stop tower buying process if present
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

    /// <summary>
    /// Called on tower upgraded
    /// </summary>
    public void OnUpgradeBtnClicked()
    {
        int price = currentUpgradeSelectedTower.Upgrade();
        UpdateMoney(-price);

        OnUpgradingUpdateUI?.Invoke(currentUpgradeSelectedTower, moneyAmount); // Update Upgrade UI panel

        OnBuyingUpdateUI?.Invoke(buyables, moneyAmount); // Update Buy UI panel
    }

    /// <summary>
    /// Called on tower sold
    /// </summary>
    public void OnSellBtnClicked()
    {
        int price = currentUpgradeSelectedTower.Sell();
        UpdateMoney(price);

        OnUpgradePanelModifyVisibility?.Invoke(); // Hide Upgrade UI panel

        // Delete the sold tower
        Destroy(currentUpgradeSelectedTower.gameObject);

        // Reinsert the deleted buildspot where the tower was initially built
        GameObject newBuildSpot = Instantiate(buildSpotPrefab, currentUpgradeSelectedTower.transform.position, Quaternion.identity);
        newBuildSpot.SetActive(false);
        buildSpots.Add(newBuildSpot);

        OnBuyingUpdateUI?.Invoke(buyables, moneyAmount); // Update Buy UI panel

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