using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Acts as a container abstract class for common data between SingleAttackType and MultipleAttackType
/// </summary>
public abstract class DamageAttackType : AttackType
{
    [SerializeField]
    protected GameObject projectilePrefab;

    protected IPropertyReadOnlyValue<float> damageAmount;

    protected IPropertyReadOnlyValue<float> attackInterval;
    protected float timeCounter; // For between attacks

    protected IPropertyReadOnlyValue<float> selectionRange;
    protected float maxFollowDistance; // If target death, how much more to travel before getting destroyed

    public override void Init()
    {
        damageAmount = GetComponent<DamageAmount>();
        attackInterval = GetComponent<AttackInterval>();

        timeCounter = attackInterval.Value;

        selectionRange = GetComponent<SelectionRange>();
        maxFollowDistance = selectionRange.Value;
    }

    /// <summary>
    /// Counting time between attacks
    /// </summary>
    protected virtual void Update()
    {
        timeCounter += Time.deltaTime;
        if (timeCounter > attackInterval.Value)
        {
            Perform();
            timeCounter = 0f;
        }
    }

    /// <summary>
    /// Perform the attack
    /// </summary>
    protected abstract void Perform();

    /// <summary>
    /// Helper method to spawn the target-follow projectile
    /// </summary>
    protected void SpawnProjectile(EnemyController currentTarget)
    {
        // Spawn a projectile
        TargetFollowController spawnedProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity, transform.parent).GetComponent<TargetFollowController>();

        maxFollowDistance = selectionRange.Value;
        spawnedProjectile.Init(currentTarget.transform, damageAmount, maxFollowDistance);
    }
}