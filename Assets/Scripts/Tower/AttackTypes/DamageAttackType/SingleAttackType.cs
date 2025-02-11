using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A tower that targets only one enemy at a time at attacks it
/// </summary>
[RequireComponent(typeof(DamageAmount))]
[RequireComponent(typeof(AttackInterval))]
public class SingleAttackType : DamageAttackType
{
    private EnemyController currentTarget;

    protected override void Update()
    {
        if (currentTarget != null)
            base.Update();
    }

    protected override void Perform()
    {
        SpawnProjectile(currentTarget);
    }

    public override void SetUp(List<EnemyController> targets)
    {
        currentTarget = targets[0];
    }

    public override void SetDown()
    {
        currentTarget = null;
    }
}
