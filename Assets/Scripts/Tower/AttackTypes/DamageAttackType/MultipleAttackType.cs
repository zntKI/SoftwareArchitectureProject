using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A tower that toggles between targets for attacking
/// </summary>
[RequireComponent(typeof(DamageAmount))]
[RequireComponent(typeof(AttackInterval))]
public class MultipleAttackType : DamageAttackType
{
    private List<EnemyController> currentTargets;
    private int currentTargetIndex = 0;

    public override void Init()
    {
        base.Init();

        currentTargets = new List<EnemyController>();
    }

    protected override void Update()
    {
        if (currentTargets.Count > 0)
            base.Update();
    }

    protected override void Perform()
    {
        currentTargetIndex++;
        if (currentTargetIndex > currentTargets.Count - 1)
        {
            currentTargetIndex = 0;
        }

        EnemyController currentTarget = currentTargets[currentTargetIndex];

        SpawnProjectile(currentTarget);
    }

    public override void SetUp(List<EnemyController> targets)
    {
        currentTargets = targets;
    }

    public override void SetDown()
    {
        currentTargets.Clear();
    }
}