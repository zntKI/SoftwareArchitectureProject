using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class encapsulating all UI elements' reference for a tower in the Buy UI panel<br></br>
/// Serializable to be visible for the inspector
/// </summary>
[Serializable]
public class TowerToBuyUIData
{
    public GameObject TowerPrefab => towerPrefab;
    public GameObject SetTowerPrefab(GameObject towerPrefab)
    {
        this.towerPrefab = towerPrefab;
        return this.towerPrefab;
    }

    private GameObject towerPrefab;

    public Image TowerSprite;

    public RectTransform TowerDetails;
    public TextMeshProUGUI AttackType;
    public TextMeshProUGUI Range;
    public TextMeshProUGUI Damage;
    public TextMeshProUGUI AttackInterval;

    public TextMeshProUGUI Price;
}

/// <summary>
/// A class encapsulating all UI elements for a tower in the Upgrade/Sell UI panel<br></br>
/// Serializable to be visible for the inspector
/// </summary>
[Serializable]
public class TowerToUpgradeUIData
{
    public TextMeshProUGUI Range;
    public TextMeshProUGUI DamageDebuff;
    public TextMeshProUGUI UpgradeBtnText;
    public Image UpgradeBtnImage;
    public Button UpgradeBtn;
    public TextMeshProUGUI UpgradePrice;

    public TextMeshProUGUI SellPrice;
}

/// <summary>
/// GameUI Singleton manager, responsible for appropriately displaying all HUD elements as well as switching between them
/// </summary>
public class GameUIManager : MonoBehaviour
{
    public static event Action OnPauseMenuInteraction;

    /// <summary>
    /// <The selected tower>
    /// </summary>
    public static event Action<GameObject> OnTowerSelection;

    public static GameUIManager Instance => instance;
    static GameUIManager instance;

    [Header("Game state UI containers")]
    [SerializeField]
    private GameObject waveUICont;
    [SerializeField]
    private GameObject buildingUICont;

    [Header("Build Manager Buying UI")]
    [SerializeField]
    private TextMeshProUGUI timeAmountText;
    [SerializeField]
    private TextMeshProUGUI moneyAmountText;

    [SerializeField]
    private List<TowerToBuyUIData> buyMenuTowers;

    [Header("Build Manager Upgrading UI")]
    [SerializeField]
    private GameObject upgradeSellPanel;

    [SerializeField]
    private TowerToUpgradeUIData towerToUpgradeUIData;

    [Header("Wave Manager UI")]
    [SerializeField]
    private TextMeshProUGUI enemiesAmountText;
    [SerializeField]
    private TextMeshProUGUI wavesAmountText;

    [Header("Pause Meny UI container")]
    [SerializeField]
    private GameObject pauseMenu;

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

        WaveManager.OnWaveStartUI += UpdateGameStateUI;
        BuildManager.OnBuildingStartUI += UpdateGameStateUI;

        BuildManager.OnTimeUpdate += UpdateBuildTime;
        BuildManager.OnMoneyUpdated += UpdateMoneyText;
        BuildManager.OnBuyingUpdateUI += UpdateBuyPanel;
        BuildManager.OnUpgradePanelModifyVisibility += ModifyUpgradeSellPanelVisibility;
        BuildManager.OnUpgradingUpdateUI += UpdateUpgradePanel;

        WaveManager.OnUpdateEnemiesAmount += UpdateEnemyAmountsText;
        WaveManager.OnUpdateWavesAmount += UpdateWavesAmountText;
    }

    void Start()
    {
        pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // Open/Close Pause Menu
        {
            OnPauseMenuInteraction?.Invoke();
            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
        }
    }

    /// <summary>
    /// Switch between the HUD for the different GameStates - Building and Wave Spawning
    /// </summary>
    void UpdateGameStateUI(GameState newState)
    {
        switch (newState)
        {
            case GameState.Building:

                buildingUICont.SetActive(true);
                upgradeSellPanel.SetActive(false); // Just in case
                waveUICont.SetActive(false);

                break;
            case GameState.Wave:

                waveUICont.SetActive(true);
                buildingUICont.SetActive(false);

                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Updates the remaining time of the build sess
    /// </summary>
    void UpdateBuildTime(int time)
    {
        timeAmountText.text = time.ToString();
    }

    /// <summary>
    /// Updates the money UI element
    /// </summary>
    void UpdateMoneyText(int newMoneyAmount)
    {
        moneyAmountText.text = newMoneyAmount.ToString();
    }

    /// <summary>
    /// Updates how many enemies have passed out of the allowed amount
    /// </summary>
    void UpdateEnemyAmountsText(int currAmount, int totalAmount)
    {
        enemiesAmountText.text = $"{currAmount}/{totalAmount}";
    }

    /// <summary>
    /// Updates the current wave num out of all
    /// </summary>
    void UpdateWavesAmountText(int currAmount, int totalAmount)
    {
        wavesAmountText.text = $"{currAmount}/{totalAmount}";
    }

    /// <summary>
    /// Handles the updating of the Tower Buy UI panel
    /// </summary>
    /// <param name="towersData">All the Towers data to display in the UI</param>
    /// <param name="currentAmountMoney">Depending on the current amount of money, modify certain UI elements</param>
    void UpdateBuyPanel(List<BuyableTower> towersData, int currentAmountMoney)
    {
        if (towersData.Count > buyMenuTowers.Count)
        {
            Debug.LogWarning("Cannot have more towers for buying than slots in the UI");
        }

        int iterator = towersData.Count < buyMenuTowers.Count ? towersData.Count : buyMenuTowers.Count;
        for (int i = 0; i < iterator; i++) // iterate through all towers and set UI elements appropriately
        {
            GameObject towerPrefab = buyMenuTowers[i].SetTowerPrefab(towersData[i].towerPrefab);

            buyMenuTowers[i].TowerSprite.sprite = towerPrefab.GetComponent<SpriteRenderer>().sprite;

            buyMenuTowers[i].TowerDetails.gameObject.SetActive(false);
            AttackType attType = towerPrefab.GetComponent<AttackType>();
            switch (attType)
            {
                case SingleAttackType:
                    buyMenuTowers[i].AttackType.text = "Single Attack";
                    break;
                case MultipleAttackType:
                    buyMenuTowers[i].AttackType.text = "Multiple Attack";
                    break;
                case DebuffAttackType:
                    buyMenuTowers[i].AttackType.text = "Debuff Attack";
                    break;
                default:
                    Debug.LogError("Invalid AttackType!");
                    return;
            }
            buyMenuTowers[i].Range.text = $"Range: {towerPrefab.GetComponent<SelectionRange>().Value}";
            // If it has such component, display detail, if not - hide it
            if (towerPrefab.TryGetComponent<DamageAmount>(out DamageAmount damageAmount))
            {
                buyMenuTowers[i].Damage.text = $"Damage: {damageAmount.Value}";
            }
            else
            {
                buyMenuTowers[i].Damage.gameObject.SetActive(false);
            }
            if (towerPrefab.TryGetComponent<AttackInterval>(out AttackInterval attackInterval))
            {
                buyMenuTowers[i].AttackInterval.text = $"Interval: {attackInterval.Value}";
            }
            else
            {
                buyMenuTowers[i].AttackInterval.gameObject.SetActive(false);
            }

            buyMenuTowers[i].Price.text = $"Price: {towersData[i].price}";
            // Depending on current amount of money, modify UI elements
            if (towersData[i].price <= currentAmountMoney)
            {
                buyMenuTowers[i].Price.color = Color.green;
                buyMenuTowers[i].TowerSprite.color = new Color(
                    buyMenuTowers[i].TowerSprite.color.r, buyMenuTowers[i].TowerSprite.color.g, buyMenuTowers[i].TowerSprite.color.b,
                    1f);
            }
            else
            {
                buyMenuTowers[i].Price.color = Color.red;
                buyMenuTowers[i].TowerSprite.color = new Color(
                    buyMenuTowers[i].TowerSprite.color.r, buyMenuTowers[i].TowerSprite.color.g, buyMenuTowers[i].TowerSprite.color.b,
                    0.4f);
            }
        }
    }

    /// <summary>
    /// Called when clicked on tower selector in UI
    /// </summary>
    /// <param name="towerDetails">The UI object to set visibility</param>
    public void SelectTower(RectTransform towerDetails)
    {
        if (towerDetails.gameObject.activeInHierarchy) // If unselecting a tower
        {
            towerDetails.gameObject.SetActive(false);
            OnTowerSelection?.Invoke(null);
            return;
        }

        // Check if other tower was selected beforehand
        foreach (var tower in buyMenuTowers)
        {
            if (tower.TowerDetails.gameObject.activeInHierarchy)
            {
                tower.TowerDetails.gameObject.SetActive(false);
            }
        }

        towerDetails.gameObject.SetActive(true);
        OnTowerSelection?.Invoke(buyMenuTowers.First(t => t.TowerDetails == towerDetails).TowerPrefab);
    }

    /// <summary>
    /// Show/Hide UI Upgrade/Sell panel
    /// </summary>
    void ModifyUpgradeSellPanelVisibility()
    {
        upgradeSellPanel.SetActive(!upgradeSellPanel.activeInHierarchy);
    }

    /// <summary>
    /// Handles the updating of the Tower Upgrade/Sell UI panel
    /// </summary>
    /// <param name="tower">Tower details</param>
    /// <param name="currentAmountMoney">Depending on the current amount of money, modify certain UI elements</param>
    void UpdateUpgradePanel(TowerController tower, int currentAmountMoney)
    {
        if (!upgradeSellPanel.activeInHierarchy) // Check just in case
        {
            ModifyUpgradeSellPanelVisibility();
        }

        TowerModel model = tower.GetComponent<TowerModel>();

        if (tower.IsLastLevel()) // Do not show tower upgrade details if last level - no more upgrading should be possible
        {
            ModifyUpgradeDetailsVisibility(false);
        }
        else
        {
            ModifyUpgradeDetailsVisibility(true);

            towerToUpgradeUIData.Range.text = $"Range: +{model.UpgradeRange} = {model.CurrentRange + model.UpgradeRange}";
            towerToUpgradeUIData.DamageDebuff.text = $"Damage/Debuff: +{model.UpgradeDamageDebuff} = {model.CurrentDamageDebuff + model.UpgradeDamageDebuff}";
            towerToUpgradeUIData.UpgradePrice.text = $"Price: {model.UpgradePrice}";

            // Depending on current amount of money, modify UI elements
            if (model.UpgradePrice <= currentAmountMoney)
            {
                towerToUpgradeUIData.UpgradeBtnText.color = new Color(
                    towerToUpgradeUIData.UpgradeBtnText.color.r, towerToUpgradeUIData.UpgradeBtnText.color.g, towerToUpgradeUIData.UpgradeBtnText.color.b,
                    1f);
                towerToUpgradeUIData.UpgradeBtnImage.color = new Color(
                    towerToUpgradeUIData.UpgradeBtnImage.color.r, towerToUpgradeUIData.UpgradeBtnImage.color.g, towerToUpgradeUIData.UpgradeBtnImage.color.b,
                    1f);

                towerToUpgradeUIData.UpgradeBtn.enabled = true;

                towerToUpgradeUIData.UpgradePrice.color = Color.green;
            }
            else
            {
                towerToUpgradeUIData.UpgradeBtnText.color = new Color(
                    towerToUpgradeUIData.UpgradeBtnText.color.r, towerToUpgradeUIData.UpgradeBtnText.color.g, towerToUpgradeUIData.UpgradeBtnText.color.b,
                    0.4f);
                towerToUpgradeUIData.UpgradeBtnImage.color = new Color(
                    towerToUpgradeUIData.UpgradeBtnImage.color.r, towerToUpgradeUIData.UpgradeBtnImage.color.g, towerToUpgradeUIData.UpgradeBtnImage.color.b,
                    0.4f);

                towerToUpgradeUIData.UpgradeBtn.enabled = false;

                towerToUpgradeUIData.UpgradePrice.color = Color.red;
            }
        }

        towerToUpgradeUIData.SellPrice.text = $"Get: {model.SellPrice}";
    }

    /// <summary>
    /// Helper method for showing/hiding Tower Upgrade details
    /// </summary>
    void ModifyUpgradeDetailsVisibility(bool shouldShow)
    {
        towerToUpgradeUIData.Range.gameObject.SetActive(shouldShow);
        towerToUpgradeUIData.DamageDebuff.gameObject.SetActive(shouldShow);
        towerToUpgradeUIData.UpgradePrice.gameObject.SetActive(shouldShow);

        towerToUpgradeUIData.UpgradeBtn.enabled = false;
    }

    void OnDestroy()
    {
        WaveManager.OnWaveStartUI -= UpdateGameStateUI;
        BuildManager.OnBuildingStartUI -= UpdateGameStateUI;

        BuildManager.OnTimeUpdate -= UpdateBuildTime;
        BuildManager.OnMoneyUpdated -= UpdateMoneyText;
        BuildManager.OnBuyingUpdateUI -= UpdateBuyPanel;
        BuildManager.OnUpgradePanelModifyVisibility -= ModifyUpgradeSellPanelVisibility;
        BuildManager.OnUpgradingUpdateUI -= UpdateUpgradePanel;

        WaveManager.OnUpdateEnemiesAmount -= UpdateEnemyAmountsText;
        WaveManager.OnUpdateWavesAmount -= UpdateWavesAmountText;
    }
}