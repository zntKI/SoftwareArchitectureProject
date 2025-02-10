using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DamageAmount))]
[RequireComponent(typeof(AttackInterval))]
public class MultipleAttackType : AttackType
{
    [SerializeField]
    private GameObject projectilePrefab;

    private DamageAmount damageAmount;

    private AttackInterval attackInterval;
    private float timeCounter;

    private List<EnemyController> currentTargets;
    private int currentTargetIndex = 0;

    private float maxFollowDistance;

    private void Start()
    {
        damageAmount = GetComponent<DamageAmount>();
        attackInterval = GetComponent<AttackInterval>();

        currentTargets = new List<EnemyController>();

        timeCounter = attackInterval.Value;

        maxFollowDistance = GetComponent<SelectionRange>().Value;
    }

    void Update()
    {
        if (currentTargets.Count > 0)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter > attackInterval.Value)
            {
                Perform();
                timeCounter = 0f;
            }
        }
    }

    private void Perform()
    {
        currentTargetIndex++;
        if (currentTargetIndex > currentTargets.Count - 1)
        {
            currentTargetIndex = 0;
        }

        EnemyController currentTarget = currentTargets[currentTargetIndex];

        // Spawn a projectile
        TargetFollowController spawnedProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity, transform.parent).GetComponent<TargetFollowController>();
        spawnedProjectile.Init(currentTarget.transform, damageAmount, maxFollowDistance);
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