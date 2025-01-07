using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DamageAmount))]
[RequireComponent(typeof(AttackInterval))]
public class SingleAttackType : AttackType
{
    private DamageAmount damageAmount;
    private AttackInterval attackInterval;

    private void Start()
    {
        damageAmount = GetComponent<DamageAmount>();
        attackInterval = GetComponent<AttackInterval>();
    }

    public override void Perform(List<GameObject> targets)
    {
        // TODO: Perform attack on single target
    }
}
