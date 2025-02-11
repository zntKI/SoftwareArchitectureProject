using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A tower that applies constant slow-down effect to the enemies that are currently in range.
/// </summary>
[RequireComponent(typeof(SlowDownAmount))]
public class DebuffAttackType : AttackType
{
    /// <summary>
    /// Color overlay to apply to an Enemy when slowed down
    /// </summary>
    [SerializeField]
    private Color enemyColorWhenSlowedDown;

    private IPropertyReadOnlyValue<float> slowDownAmount;

    public override void Init()
    {
        slowDownAmount = GetComponent<SlowDownAmount>();
    }

    public override void SetUp(List<EnemyController> targets)
    {
        foreach (EnemyController target in targets)
        {
            target.ShouldBeSlowedDown(slowDownAmount, enemyColorWhenSlowedDown);
        }
    }

    public override void SetDown()
    {
    }
}