using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(EnemyModel))]
[RequireComponent(typeof(MovementBehaviour))]
public class EnemyController : MonoBehaviour
{
    public static event Action<EnemyController> OnDied;
    public static event Action<int> OnMoneyDropped;

    public static event Action<EnemyController> OnReachedEnd;

    private EnemyModel model;
    private EnemyView view;

    private MovementBehaviour movementBehaviour;

    private bool isSlowedDown;

    void Start()
    {
        model = GetComponent<EnemyModel>();
        model.OnHealthZero += Died;

        view = GetComponent<EnemyView>();

        movementBehaviour = GetComponent<MovementBehaviour>();
        movementBehaviour.OnReachedEnd += ReachedEnd;
    }

    void Update()
    {
        movementBehaviour.DoMoving();
    }

    public void TakeDamage(DamageAmount damageAmount)
    {
        model.UpdateHealth(-damageAmount.Damage);
        view.CheckHealth(model.Health, model.InitialHealth);
    }

    public void ShouldBeSlowedDown(SlowDownAmount slowDownAmount)
    {
        if (!isSlowedDown)
        {
            isSlowedDown = true;

            model.UpdateSpeed(slowDownAmount.SlowDownPercantage);
        }
    }

    public void OnTargetZoneLeave()
    {
        if (isSlowedDown)
        {
            isSlowedDown = false;

            model.ResetSpeed();
        }
    }

    void ReachedEnd()
    {
        OnReachedEnd?.Invoke(this);

        Destroy(gameObject);
        // Also fire an event to increase the counter of enemies passed?
    }

    void Died()
    {
        view.SpawnMoneyParticle(model.Money);

        Destroy(gameObject);

        OnDied?.Invoke(this);
        OnMoneyDropped?.Invoke(model.Money);
        // Also fire an event to increase money?
    }

    void OnDestroy()
    {
        if (movementBehaviour != null)
            movementBehaviour.OnReachedEnd -= ReachedEnd;
    }
}
