using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAmount : MonoBehaviour, IPropertyValue<float>
{
    [SerializeField]
    private float damage = 1f;

    public float Value => damage;

    public void ModifyValue(float value)
    {
        damage = value;
    }

    //public float Damage => damage;
}
