using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionRange : MonoBehaviour, IPropertyValue<float>
{
    [SerializeField]
    private float range = 5f;

    public float Value { get => range; }

    public void ModifyValue(float value)
    {
        range = value;
    }

    //public float Range => range;
}