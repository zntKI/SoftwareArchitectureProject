using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TargetSelector))]
[RequireComponent(typeof(AttackType))]
public class TowerController : MonoBehaviour
{
    private TargetSelector towerSelector;
    private AttackType attackType;

    private void Start()
    {
        towerSelector = GetComponent<TargetSelector>();
        attackType = GetComponent<AttackType>();
    }

    private void Update()
    {
        List<ITarget> targets = towerSelector.SelectTarget();

        if (targets.Count != 0)
        {
            attackType.Perform(targets);
        }
    }
}
