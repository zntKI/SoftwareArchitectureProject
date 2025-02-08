using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSpotController : MonoBehaviour
{
    public static event Action<Transform> OnBuildSpotClicked;

    void OnMouseDown()
    {
        Destroy(gameObject);

        // Fire an event to build a tower
        OnBuildSpotClicked?.Invoke(transform);
    }
}