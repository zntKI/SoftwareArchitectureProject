using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementBehaviour : MonoBehaviour
{
    public static event Action<GameObject> OnReachedEnd;

    protected void ReachedEnd()
    {
        OnReachedEnd?.Invoke(gameObject);
    }

    public abstract void DoMoving();
}
