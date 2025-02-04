using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollowController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;

    private Transform target;
    private DamageAmount targetDamageAmount;

    void Start()
    {
    }

    public void Init(Transform targetToSet, DamageAmount damageToDeal)
    {
        target = targetToSet;
        targetDamageAmount = damageToDeal;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.up = dir;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyController>(out EnemyController enemy))
        {
            Debug.Log("TakeDamage");
            enemy.TakeDamage(targetDamageAmount);
            Destroy(gameObject);
        }
    }
}
