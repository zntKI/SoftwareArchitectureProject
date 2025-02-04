using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementBehaviour : MonoBehaviour
{
    public event Action OnReachedEnd;

    protected void ReachedEnd()
    {
        OnReachedEnd?.Invoke();
    }

    public abstract void DoMoving();
}
