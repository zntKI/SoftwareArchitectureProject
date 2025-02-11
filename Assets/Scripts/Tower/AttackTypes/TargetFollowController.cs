using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class defining the behaviour of all target-follow projectiles
/// </summary>
public class TargetFollowController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;

    private FollowState followState = FollowState.None;

    private Vector3 startPos;
    private float maxFollowRange;

    private Transform target;
    private IPropertyReadOnlyValue<float> targetDamageAmount;

    private Vector3 dir;

    void Awake()
    {
        EnemyController.OnDied += CheckIfSwitchFollowState;
    }

    /// <summary>
    /// Called right after being created
    /// </summary>
    public void Init(Transform targetToSet, IPropertyReadOnlyValue<float> damageToDeal, float maxFollowRange)
    {
        followState = FollowState.TargetFollow;

        startPos = transform.position;

        this.maxFollowRange = maxFollowRange;

        target = targetToSet;
        targetDamageAmount = damageToDeal;
    }

    void Update()
    {
        switch (followState)
        {
            case FollowState.None:
                break;
            case FollowState.TargetFollow: // Follow target by facing towards it

                dir = (target.position - transform.position).normalized;
                transform.up = dir;

                break;
            case FollowState.DirectionFollow: // Follow a given direction

                if ((transform.position - startPos).magnitude > maxFollowRange) // If reached max distance
                {
                    Destroy(gameObject);
                    return;
                }

                break;
            default:
                break;
        }

        transform.position += dir * moveSpeed * Time.deltaTime;
    }
    
    /// <summary>
    /// Called when Enemy dies and checks if the Enemy is the target, switch to the DirectionFollow state
    /// </summary>
    void CheckIfSwitchFollowState(EnemyController controller)
    {
        // Check if the same enemy has died
        if (controller.transform == target)
        {
            followState = FollowState.DirectionFollow;
        }
    }

    /// <summary>
    /// Registers collision with Enemy and inflicted damage
    /// </summary>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyController>(out EnemyController enemy))
        {
            enemy.TakeDamage(targetDamageAmount);
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        EnemyController.OnDied -= CheckIfSwitchFollowState;
    }
}

public enum FollowState
{
    None,
    TargetFollow, // Follow directly target
    DirectionFollow, // Follow just direction if target destroyed
}