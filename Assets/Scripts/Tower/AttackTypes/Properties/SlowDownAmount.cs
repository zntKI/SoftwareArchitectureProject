using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownAmount : MonoBehaviour
{
    [SerializeField]
    private float slowDownPercantage = 1f;

    public float SlowDownPercantage => slowDownPercantage;
}
