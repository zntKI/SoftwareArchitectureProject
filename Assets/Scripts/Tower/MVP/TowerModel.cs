using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Part of the MVP pattern for Tower<br></br>
/// Handles all data regarding Tower
/// </summary>
public class TowerModel : MonoBehaviour
{
    public int BuyPrice => buyPrice;

    public int UpgradePrice => upgradePrice;
    public float UpgradeRange => rangeUpgradePerLevel;
    public float UpgradeDamageDebuff => damageOrDebuffUpgradeLevel;

    public int SellPrice => sellPrice;

    public float CurrentRange => range.Value;
    public float CurrentDamageDebuff => damageAmount == null ? debuffAmount.Value
        : damageAmount.Value;

    [Header("Buying")]
    [SerializeField]
    private int buyPrice;

    [Header("Upgrading")]
    [SerializeField]
    private int upgradePrice;
    [SerializeField]
    private float rangeUpgradePerLevel;
    [SerializeField]
    private float damageOrDebuffUpgradeLevel;

    private SelectionRange range;
    /// <summary>
    /// Either this or 'debuffAmount' will be null, dependin on what type of tower this is
    /// </summary>
    private DamageAmount damageAmount;
    /// <summary>
    /// Either this or 'damageAmount' will be null, dependin on what type of tower this is
    /// </summary>
    private SlowDownAmount debuffAmount;

    [Header("Selling")]
    [SerializeField]
    private int sellPrice;

    [Header("LevelUp")]
    [SerializeField]
    private int priceAddAmount;

    /// <summary>
    /// Need to be called before Start
    /// </summary>
    public void Init()
    {
        range = GetComponent<SelectionRange>();

        if (!TryGetComponent<DamageAmount>(out damageAmount)
            && !TryGetComponent<SlowDownAmount>(out debuffAmount))
        {
            Debug.LogError("Each tower should have either 'DamageAmount' or 'SlowDownAmount' component attached to it!");
            return;
        }

        if (priceAddAmount == 0)
        {
            priceAddAmount = upgradePrice;
        }
    }

    /// <summary>
    /// Called on Tower Upgrade to update Tower data accordingly
    /// </summary>
    public void Upgrade()
    {
        range.ModifyValue(upgradePrice);
        if (damageAmount == null)
            debuffAmount.ModifyValue(damageOrDebuffUpgradeLevel);
        else
            damageAmount.ModifyValue(damageOrDebuffUpgradeLevel);

        upgradePrice += priceAddAmount;
        sellPrice += priceAddAmount;
    }
}