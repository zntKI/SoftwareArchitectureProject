using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownAmount : MonoBehaviour, IPropertyValue<float>
{
    [SerializeField]
    private float slowDownPercantage = 1f;

    public float Value => slowDownPercantage;

    public void ModifyValue(float value)
    {
        slowDownPercantage = value;
    }

    //public float SlowDownPercantage => slowDownPercantage;
}
