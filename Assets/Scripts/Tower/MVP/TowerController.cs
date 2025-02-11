using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Part of the MVP pattern for Tower<br></br>
/// Acts as a middleman between TowerModel and TowerView, as well as TargetSelector and AttackType, handing out tasks
/// </summary>
[RequireComponent(typeof(TowerModel))]
[RequireComponent(typeof(TowerView))]
[RequireComponent(typeof(TargetSelector))]
[RequireComponent(typeof(AttackType))]
public class TowerController : MonoBehaviour
{
    public static event Action<TowerController> OnTowerClicked;

    private TowerModel model;
    private TowerView view;

    private TargetSelector towerSelector;
    private AttackType attackType;

    void Start()
    {
        if (model == null && view == null
            && towerSelector == null && attackType == null) // If not instantiated through BuildManager but one of the test scenes
        {
            Init();
        }
    }

    /// <summary>
    /// Need to be called before Start
    /// </summary>
    public void Init()
    {
        model = GetComponent<TowerModel>();
        model.Init();
        view = GetComponent<TowerView>();
        view.Init();

        towerSelector = GetComponent<TargetSelector>();
        towerSelector.Init();
        attackType = GetComponent<AttackType>();
        attackType.Init();
    }

    void Update()
    {
        List<EnemyController> targets = towerSelector.SelectTarget();

        if (targets.Count != 0)
        {
            attackType.SetUp(targets);
        }
        else
        {
            attackType.SetDown();
        }
    }

    /// <summary>
    /// Retrieves if Tower is last level
    /// </summary>
    public bool IsLastLevel()
        => view.IsAtLastSprite();

    public void ModifyOutlineVisibility()
        => view.UpdateOutline();

    /// <summary>
    /// Hands out Upgrade tasks
    /// </summary>
    /// <returns>Price for the upgrade</returns>
    public int Upgrade()
    {
        int upgradePrice = model.UpgradePrice;

        model.Upgrade();
        view.Upgrade();

        return upgradePrice;
    }

    /// <returns>Price for the selling</returns>
    public int Sell()
    {
        return model.SellPrice;
    }

    void OnMouseDown()
    {
        OnTowerClicked?.Invoke(this);
    }
}