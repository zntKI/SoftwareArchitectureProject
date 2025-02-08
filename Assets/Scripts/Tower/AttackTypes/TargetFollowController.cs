using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollowController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;

    private FollowState followState = FollowState.None;

    private Vector3 startPos;
    private float maxFollowRange;

    private Transform target;
    private DamageAmount targetDamageAmount;

    private Vector3 dir;

    void Awake()
    {
        EnemyController.OnDied += CheckIfSwitchFollowState;
    }

    void Start()
    {
    }

    public void Init(Transform targetToSet, DamageAmount damageToDeal, float maxFollowRange)
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
            case FollowState.TargetFollow:

                dir = (target.position - transform.position).normalized;
                transform.up = dir;

                break;
            case FollowState.DirectionFollow:

                if ((transform.position - startPos).magnitude > maxFollowRange)
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
    
    void CheckIfSwitchFollowState(EnemyController controller)
    {
        // Check if the same enemy has died
        if (controller.transform == target)
        {
            followState = FollowState.DirectionFollow;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyController>(out EnemyController enemy))
        {
            //Debug.Log("TakeDamage");
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
    TargetFollow,
    DirectionFollow,
}