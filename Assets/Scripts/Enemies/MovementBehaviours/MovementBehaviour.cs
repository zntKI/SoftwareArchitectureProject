using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract behaviour that acts as a blueprint for different types of Enemy movement
/// </summary>
public abstract class MovementBehaviour : MonoBehaviour
{
    /// <summary>
    /// Event called when Enemy reaches end of path
    /// </summary>
    public event Action OnReachedEnd;

    protected void ReachedEnd()
    {
        OnReachedEnd?.Invoke();
    }

    public abstract void DoMoving();
}