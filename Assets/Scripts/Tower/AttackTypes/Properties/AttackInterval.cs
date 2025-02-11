using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tower Property that indicates the time interval between each attack of a Tower
/// </summary>
public class AttackInterval : MonoBehaviour, IPropertyReadOnlyValue<float>, IPropertyModifiableValue<float>
{
    [SerializeField]
    private float attackIntervalSec = 1f;

    public float Value => attackIntervalSec;

    public void ModifyValue(float value)
    {
        attackIntervalSec = value;
    }
}