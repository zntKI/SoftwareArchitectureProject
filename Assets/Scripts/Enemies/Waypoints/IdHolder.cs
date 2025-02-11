using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used by Enemy waypoints to set a given follow order
/// </summary>
public class IdHolder : MonoBehaviour
{
    public int Id => id;

    [SerializeField]
    int id = 0;
}