using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetSelector : MonoBehaviour
{
    public abstract List<ITarget> SelectTarget();
}