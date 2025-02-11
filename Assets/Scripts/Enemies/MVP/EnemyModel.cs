using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class encapsulating all functionalities regarding Enemy Speed<br></br>
/// Serializable to be visible for the inspector
/// </summary>
[Serializable]
public class Speed
{
    public float Value => moveSpeed;

    [SerializeField]
    private float moveSpeed;
    private float initialSpeed;

    public void Init()
    {
        initialSpeed = moveSpeed;
    }

    public void SetSpeed(float portionOfOriginalSpeed)
    {
        moveSpeed = moveSpeed * portionOfOriginalSpeed;
    }

    public void ResetSpeed()
    {
        moveSpeed = initialSpeed;
    }
}

/// <summary>
/// A class encapsulating all functionalities regarding Enemy Health<br></br>
/// Serializable to be visible for the inspector
/// </summary>
[Serializable]
public class Health
{
    public float Value => healthAmount;

    [SerializeField]
    private float healthAmount;
    private float initialHealth;

    public void Init()
    {
        initialHealth = healthAmount;
    }

    public float GetInitialHealth()
    {
        return initialHealth;
    }

    public float SetHealth(float modifyAmount)
    {
        var newHealthAmount = healthAmount + modifyAmount;
        healthAmount = Mathf.Clamp(newHealthAmount, 0, initialHealth);

        return healthAmount;
    }
}

/// <summary>
/// A class encapsulating all functionalities regarding Enemy Money<br></br>
/// Serializable to be visible for the inspector
/// </summary>
[Serializable]
public class Money
{
    public int Value => moneyAmount;

    [SerializeField]
    private int moneyAmount;
}

/// <summary>
/// Part of the MVP pattern for Enemy<br></br>
/// Handles all data regarding Enemy
/// </summary>
[RequireComponent(typeof(EnemyView))]
public class EnemyModel : MonoBehaviour
{
    public float Speed => speed.Value;

    public float Health => health.Value;
    public float InitialHealth => health.GetInitialHealth();

    public int Money => money.Value;

    public event Action OnHealthZero;

    [SerializeField]
    private Speed speed;

    [SerializeField]
    private Health health;

    [SerializeField]
    private Money money;

    void Awake()
    {
        health.Init();
        speed.Init();
    }

    /// <summary>
    /// Updates the speed by a given amount
    /// </summary>
    /// <param name="modifyAmount">Value between 0 and 1</param>
    public void UpdateSpeed(float portionOfOriginalSpeed)
    {
        speed.SetSpeed(portionOfOriginalSpeed);
    }

    /// <summary>
    /// Sets speed to initial speed
    /// </summary>
    public void ResetSpeed()
    {
        speed.ResetSpeed();
    }

    /// <summary>
    /// Updates the health by a given amount
    /// </summary>
    /// <param name="modifyAmount">Negative value if should reduce, positive if increase</param>
    public void UpdateHealth(float modifyAmount)
    {
        if (health.SetHealth(modifyAmount) == 0)
        {
            OnHealthZero?.Invoke();
        }
    }
}