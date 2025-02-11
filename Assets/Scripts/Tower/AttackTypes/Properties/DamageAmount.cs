using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tower Property that indicates the damage that a tower inflicts on Enemies
/// </summary>
public class DamageAmount : MonoBehaviour, IPropertyReadOnlyValue<float>, IPropertyModifiableValue<float>
{
    [SerializeField]
    private float damage = 1f;

    public float Value => damage;

    public void ModifyValue(float value)
    {
        damage = value;
    }
}
