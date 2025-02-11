using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tower Property that indicates the selection range of Tower
/// </summary>
public class SelectionRange : MonoBehaviour, IPropertyReadOnlyValue<float>, IPropertyModifiableValue<float>
{
    [SerializeField]
    private float range = 5f;

    public float Value { get => range; }

    public void ModifyValue(float value)
    {
        range = value;
    }
}