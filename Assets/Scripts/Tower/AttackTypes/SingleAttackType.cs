using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DamageAmount))]
[RequireComponent(typeof(AttackInterval))]
public class SingleAttackType : AttackType
{
    [SerializeField]
    private GameObject projectilePrefab;

    private DamageAmount damageAmount;

    private AttackInterval attackInterval;
    private float timeCounter;

    private EnemyController currentTarget;

    private void Start()
    {
        damageAmount = GetComponent<DamageAmount>();
        attackInterval = GetComponent<AttackInterval>();

        timeCounter = attackInterval.AttackIntervalSec;
    }

    void Update()
    {
        if (currentTarget != null)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter > attackInterval.AttackIntervalSec)
            {
                Perform();
                timeCounter = 0f;
            }
        }
    }

    private void Perform()
    {
        // Spawn a projectile
        TargetFollowController spawnedProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity, transform.parent).GetComponent<TargetFollowController>();
        spawnedProjectile.Init(currentTarget.transform, damageAmount);
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
