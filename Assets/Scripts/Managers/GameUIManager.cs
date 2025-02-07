using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct TowerToBuyUIData
{
    public Image TowerSprite;

    public RectTransform TowerDetails;
    public TextMeshProUGUI AttackType;
    public TextMeshProUGUI Range;
    public TextMeshProUGUI Damage;
    public TextMeshProUGUI AttackInterval;

    public TextMeshProUGUI Price;
}

public class GameUIManager : MonoBehaviour
{
    public static event Action OnPauseMenuInteraction;

    public static GameUIManager Instance => instance;
    static GameUIManager instance;

    [Header("Game state UI containers")]
    [SerializeField]
    private RectTransform waveUICont;
    [SerializeField]
    private RectTransform buildingUICont;

    [Header("Build Manager UI")]
    [SerializeField]
    private TextMeshProUGUI timeAmountText;
    [SerializeField]
    private TextMeshProUGUI moneyAmountText;

    [SerializeField]
    private List<TowerToBuyUIData> buyMenuTowers;

    [Header("Wave Manager UI")]
    [SerializeField]
    private TextMeshProUGUI enemiesAmountText;
    [SerializeField]
    private TextMeshProUGUI wavesAmountText;

    [Header("Pause Meny UI container")]
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

        WaveManager.OnWaveStartUI += UpdateGameStateUI;
        BuildManager.OnBuildingStartUI += UpdateGameStateUI;

        BuildManager.OnTimeUpdate += UpdateBuildTime;
        BuildManager.OnMoneyUpdated += UpdateMoneyText;
        BuildManager.OnBuyingSetupUI += UpdateBuyPanel;

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

    void UpdateGameStateUI(GameState newState)
    {
        switch (newState)
        {
            case GameState.Building:

                buildingUICont.gameObject.SetActive(true);
                waveUICont.gameObject.SetActive(false);

                break;
            case GameState.Wave:

                waveUICont.gameObject.SetActive(true);
                buildingUICont.gameObject.SetActive(false);

                break;
            default:
                break;
        }
    }

    void UpdateBuildTime(int time)
    {
        timeAmountText.text = time.ToString();
    }

    void UpdateMoneyText(int newMoneyAmount)
    {
        moneyAmountText.text = newMoneyAmount.ToString();
    }

    void UpdateBuyPanel(List<BuyableTower> towersData, int currentAmountMoney)
    {
        if (towersData.Count > buyMenuTowers.Count)
        {
            Debug.LogError("Cannot have more towers for buying than slots in the UI");
            return;
        }

        for (int i = 0; i < towersData.Count; i++)
        {
            GameObject towerPrefab = towersData[i].towerPrefab;

            buyMenuTowers[i].TowerSprite.sprite = towerPrefab.GetComponent<SpriteRenderer>().sprite;

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
            buyMenuTowers[i].Range.text = $"Range: {towerPrefab.GetComponent<SelectionRange>().Range}";
            if (towerPrefab.TryGetComponent<DamageAmount>(out DamageAmount damageAmount))
            {
                buyMenuTowers[i].Damage.text = $"Damage: {damageAmount.Damage}";
            }
            else
            {
                buyMenuTowers[i].Damage.gameObject.SetActive(false);
            }
            if (towerPrefab.TryGetComponent<AttackInterval>(out AttackInterval attackInterval))
            {
                buyMenuTowers[i].AttackInterval.text = $"Interval: {attackInterval.AttackIntervalSec}";
            }
            else
            {
                buyMenuTowers[i].AttackInterval.gameObject.SetActive(false);
            }

            buyMenuTowers[i].Price.text = $"Price: {towersData[i].price}";
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
        WaveManager.OnWaveStartUI -= UpdateGameStateUI;
        BuildManager.OnBuildingStartUI -= UpdateGameStateUI;

        BuildManager.OnTimeUpdate -= UpdateBuildTime;
        BuildManager.OnMoneyUpdated -= UpdateMoneyText;
        BuildManager.OnBuyingSetupUI -= UpdateBuyPanel;

        WaveManager.OnUpdateEnemiesAmount -= UpdateEnemyAmountsText;
        WaveManager.OnUpdateWavesAmount -= UpdateWavesAmountText;
    }
}