using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SlowDownAmount))]
public class DebuffAttackType : AttackType
{
    SlowDownAmount slowDownAmount;

    private void Start()
    {
        slowDownAmount = GetComponent<SlowDownAmount>();
    }

    public override void SetUp(List<EnemyController> targets)
    {
        foreach (EnemyController target in targets)
        {
            target.ShouldBeSlowedDown(slowDownAmount);
        }
    }

    public override void SetDown()
    {
    }
}