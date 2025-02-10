using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInterval : MonoBehaviour, IPropertyValue<float>
{
    [SerializeField]
    private float attackIntervalSec = 1f;

    public float Value => attackIntervalSec;

    public void ModifyValue(float value)
    {
        attackIntervalSec = value;
    }

    //public float AttackIntervalSec => attackIntervalSec;
}
