using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdHolder : MonoBehaviour
{
    public int Id => id;

    [SerializeField]
    int id = 0;
}
