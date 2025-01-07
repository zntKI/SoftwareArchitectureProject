using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetSelector : MonoBehaviour
{
    protected List<GameObject> selectedTargets;

    public abstract List<GameObject> SelectTarget();
}