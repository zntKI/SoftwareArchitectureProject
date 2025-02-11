using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tower Property that indicates the slow-down amount that a tower inflicts on Enemies
/// </summary>
public class SlowDownAmount : MonoBehaviour, IPropertyReadOnlyValue<float>, IPropertyModifiableValue<float>
{
    [SerializeField]
    private float slowDownPercantage = 1f;

    public float Value => slowDownPercantage;

    public void ModifyValue(float value)
    {
        slowDownPercantage = value;
    }
}
