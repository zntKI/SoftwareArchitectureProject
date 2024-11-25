using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInterval : MonoBehaviour
{
    [SerializeField]
    private float attackIntervalSec = 1f;

    public float AttackIntervalSec => attackIntervalSec;
}
