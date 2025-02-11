using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Part of the MVP pattern for Enemy<br></br>
/// Acts as a middleman between EnemyModel and EnemyView, handing out tasks
/// </summary>
[RequireComponent(typeof(EnemyModel))]
[RequireComponent(typeof(EnemyView))]
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

    /// <summary>
    /// Need to be called before Start
    /// </summary>
    public void Init()
    {
        model = GetComponent<EnemyModel>();
        model.OnHealthZero += Died;

        view = GetComponent<EnemyView>();
        view.Init();

        movementBehaviour = GetComponent<MovementBehaviour>();
        movementBehaviour.OnReachedEnd += ReachedEnd;
    }

    void Update()
    {
        movementBehaviour.DoMoving();
    }

    /// <summary>
    /// Registers damage amount from towers' attacks
    /// </summary>
    public void TakeDamage(IPropertyReadOnlyValue<float> damageAmount)
    {
        model.UpdateHealth(-damageAmount.Value);
        view.CheckHealth(model.Health, model.InitialHealth);
    }

    /// <summary>
    /// Registers slow-down effect from 'Debuff' towers
    /// </summary>
    /// <param name="slowDownColor">Color overlay that resembles the slow-down effect</param>
    public void ShouldBeSlowedDown(IPropertyReadOnlyValue<float> slowDownAmount, Color slowDownColor)
    {
        if (!isSlowedDown) // Do not reapply the same effect more than once
        {
            isSlowedDown = true;

            model.UpdateSpeed(slowDownAmount.Value);
            view.UpdateColor(slowDownColor);
        }
    }

    /// <summary>
    /// When having left tower target zone, checks to see if it has been slowed down beforehand
    /// </summary>
    public void OnTargetZoneLeave()
    {
        if (isSlowedDown)
        {
            isSlowedDown = false;

            model.ResetSpeed();
            view.ResetColor();
        }
    }

    /// <summary>
    /// Registers an Event from MovementBehaviour that Enemy has reached the end and then destroys it
    /// </summary>
    void ReachedEnd()
    {
        OnReachedEnd?.Invoke(this);

        Destroy(gameObject);
    }

    /// <summary>
    /// Registers an Event when the Enemy health has reached zero
    /// </summary>
    void Died()
    {
        view.SpawnMoneyParticle(model.Money);

        Destroy(gameObject);

        // First do other calls before enemy death
        OnMoneyDropped?.Invoke(model.Money); // Add to Player's money amount

        OnDied?.Invoke(this); // Register Enemy death
    }

    void OnDestroy()
    {
        if (movementBehaviour != null) // First check if movement behaviour has been destroyed before that
            movementBehaviour.OnReachedEnd -= ReachedEnd;
    }
}
