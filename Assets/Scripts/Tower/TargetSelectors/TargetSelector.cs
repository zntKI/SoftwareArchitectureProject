using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetSelector : MonoBehaviour
{
    protected List<EnemyController> selectedTargets;

    void Awake()
    {
        EnemyController.OnDied += CheckIfShouldRemoveFromCollection;
    }

    protected virtual void CheckIfShouldRemoveFromCollection(EnemyController enemy)
    {
        if (selectedTargets.Contains(enemy))
        {
            selectedTargets.Remove(enemy);
        }
    }

    public abstract List<EnemyController> SelectTarget();

    void OnDestroy() 
    {
        EnemyController.OnDied -= CheckIfShouldRemoveFromCollection;
    }
}