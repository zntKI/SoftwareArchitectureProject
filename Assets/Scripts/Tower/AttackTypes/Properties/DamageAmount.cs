using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAmount : MonoBehaviour
{
    [SerializeField]
    private float damage = 1f;

    public float Damage => damage;
}
