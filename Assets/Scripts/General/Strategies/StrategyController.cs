using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Template class that all strategy controllers derive from
/// </summary>
public abstract class StrategyController : MonoBehaviour
{
    public event Action OnStrategyEnabled;
    public event Action OnStrategyDisabled;

    void Awake()
    {
        Strategy.OnStrategyEnable += EnableStrategy;
        Strategy.OnStrategyDisable += DisableStrategy;
    }

    public abstract void EnableStrategy(Strategy strategy);
    public abstract void DisableStrategy(Strategy strategy);

    protected void StrategyEnabled()
    {
        OnStrategyEnabled?.Invoke();
    }

    protected void StrategyDisabled()
    {
        OnStrategyDisabled?.Invoke();
    }

    void OnDestroy()
    {
        Strategy.OnStrategyEnable -= EnableStrategy;
        Strategy.OnStrategyDisable -= DisableStrategy;
    }
}
