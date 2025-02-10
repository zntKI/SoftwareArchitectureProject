using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        model = GetComponent<TowerModel>();
        view = GetComponent<TowerView>();

        towerSelector = GetComponent<TargetSelector>();
        attackType = GetComponent<AttackType>();
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

    public bool IsLastLevel()
        => view.IsAtLastSprite();

    public void ModifyOutlineVisibility()
        => view.UpdateOutline();

    public int Upgrade()
    {
        int upgradePrice = model.UpgradePrice;

        model.Upgrade();
        view.Upgrade();

        return upgradePrice;


    }

    public int Sell()
    {
        return model.SellPrice;
    }

    void OnMouseDown()
    {
        OnTowerClicked?.Invoke(this);
    }
}