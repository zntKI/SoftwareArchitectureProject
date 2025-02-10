using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used only when the value of the property is to be retrieved
/// and modifying is not allowed
/// </summary>
//public interface IReadOnlyValue<T>
//{
//    T Value { get; }
//}

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

[Serializable]
public class Money
{
    public int Value => moneyAmount;

    [SerializeField]
    private int moneyAmount;
}

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

    void Start()
    {
    }

    /// <summary>
    /// Updates the speed by a given amount
    /// </summary>
    /// <param name="modifyAmount">Value between 0 and 1</param>
    public void UpdateSpeed(float portionOfOriginalSpeed)
    {
        speed.SetSpeed(portionOfOriginalSpeed);
    }

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