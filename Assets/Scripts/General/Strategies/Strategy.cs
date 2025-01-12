using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Template class that all strategies derive from
/// </summary>
public abstract class Strategy : MonoBehaviour
{
    public static event Action<Strategy> OnStrategyEnable;
    public static event Action<Strategy> OnStrategyDisable;

    void OnEnable()
    {
        OnStrategyEnable?.Invoke(this);
    }

    void OnDisable()
    {
        OnStrategyDisable?.Invoke(this);
    }
}
