using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackType : MonoBehaviour
{
    public abstract void SetUp(List<EnemyController> targets);
    public abstract void SetDown();
}